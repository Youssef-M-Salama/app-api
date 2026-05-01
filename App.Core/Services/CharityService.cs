using App.Core.Domain.Entities;
using App.Core.Domain.RepositoryContracts;
using App.Core.DTOs.Request;
using App.Core.DTOs.Response;
using App.Core.DTOs.ResultPattern;
using App.Core.Enums;
using App.Core.ServiceContracts;
using Microsoft.Extensions.Logging;

namespace App.Core.Services
{
    /// <summary>
    /// Handles all business logic for authenticated charity users.
    /// </summary>
    public class CharityService : ICharityService
    {
        private readonly ICharityRepository _charityRepository;
        private readonly ICharityNeedRepository _charityNeedRepository;
        private readonly INeedApplicationRepository _needApplicationRepository;
        private readonly IOfferApplicationRepository _offerApplicationRepository;
        private readonly IOfferRepository _offerRepository;
        private readonly IProfileRepository _profileRepository;
        private readonly IFileService _fileService;
        private readonly IEmailService _emailService;
        private readonly ILogger<CharityService> _logger;

        private const int MaxPageSize = 50;

        public CharityService(
            ICharityRepository charityRepository,
            ICharityNeedRepository charityNeedRepository,
            INeedApplicationRepository needApplicationRepository,
            IOfferApplicationRepository offerApplicationRepository,
            IOfferRepository offerRepository,
            IProfileRepository profileRepository,
            IFileService fileService,
            IEmailService emailService,
            ILogger<CharityService> logger)
        {
            _charityRepository = charityRepository;
            _charityNeedRepository = charityNeedRepository;
            _needApplicationRepository = needApplicationRepository;
            _offerApplicationRepository = offerApplicationRepository;
            _offerRepository = offerRepository;
            _profileRepository = profileRepository;
            _fileService = fileService;
            _emailService = emailService;
            _logger = logger;
        }

        // =========================================================
        // DASHBOARD
        // =========================================================

        /// <inheritdoc/>
        public async Task<ServiceResult<CharityDashboardResponseDTO>> GetDashboardAsync(Guid userId)
        {
            try
            {
                var charity = await _profileRepository.GetCharityByUserIdAsync(userId);
                if (charity is null)
                    return ServiceResult<CharityDashboardResponseDTO>
                        .NotFound("ملف الجمعية غير موجود.");

                // Run count queries sequentially to avoid DbContext threading issues
                var needCounts = await _charityNeedRepository
                    .GetNeedCountsByCharityIdAsync(charity.CharityId);

                var receivedCounts = await _needApplicationRepository
                    .GetReceivedCountsByCharityIdAsync(charity.CharityId);

                var sentCounts = await _offerApplicationRepository
                    .GetSentCountsByCharityIdAsync(charity.CharityId);

                var data = new CharityDashboardResponseDTO
                {
                    // CharityNeed counts: 0 (Pending), 1 (Approved), 2 (Rejected), 3 (Fulfilled)
                    TotalCharityNeeds = needCounts.Total,
                    PendingCharityNeeds = needCounts.Pending,
                    ApprovedCharityNeeds = needCounts.Approved,
                    RejectedCharityNeeds = needCounts.Rejected,
                    FulfilledCharityNeeds = needCounts.Fulfilled,

                    // NeedApplications received: 0 (Pending), 1 (Accepted), 2 (Rejected)
                    TotalNeedApplicationsReceived = receivedCounts.Total,
                    PendingNeedApplicationsReceived = receivedCounts.Pending,
                    AcceptedNeedApplicationsReceived = receivedCounts.Accepted,
                    RejectedNeedApplicationsReceived = receivedCounts.Rejected,

                    // OfferApplications sent: 0 (Pending), 1 (Accepted), 2 (Rejected)
                    TotalOfferApplicationsSent = sentCounts.Total,
                    PendingOfferApplicationsSent = sentCounts.Pending,
                    AcceptedOfferApplicationsSent = sentCounts.Accepted,
                    RejectedOfferApplicationsSent = sentCounts.Rejected
                };

                return ServiceResult<CharityDashboardResponseDTO>
                    .Success("تم استرجاع إحصائيات لوحة التحكم بنجاح", data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in CharityService");
                return ServiceResult<CharityDashboardResponseDTO>
                    .Internal("حدث خطأ غير متوقع", new { message = ex.Message });
            }
        }

        // =========================================================
        // CHARITY NEED — CRUD
        // =========================================================

        /// <inheritdoc/>
        public async Task<ServiceResult<CharityNeedDetailResponseDTO>> CreateCharityNeedAsync(
            Guid userId,
            CreateCharityNeedRequestDTO request)
        {
            try
            {
                var charity = await _profileRepository.GetCharityByUserIdAsync(userId);
                if (charity is null)
                    return ServiceResult<CharityNeedDetailResponseDTO>
                        .NotFound("ملف الجمعية غير موجود.");

                if (!charity.IsVerified || !charity.IsActive)
                    return ServiceResult<CharityNeedDetailResponseDTO>
                        .Forbidden("يجب أن يكون حساب الجمعية الخاص بك مفعلاً ونشطاً لتتمكن من نشر الاحتياجات.");

                // Handle optional image upload
                string? imagePath = null;
                if (request.ProductImage is not null)
                {
                    var saveResult = await _fileService
                        .SaveImageAsync(request.ProductImage, ImageFolder.Needs);

                    if (!saveResult.Response.Success)
                        return ServiceResult<CharityNeedDetailResponseDTO>
                            .BadRequest(saveResult.Response.Message,
                                        saveResult.Response.Error?.Details);

                    imagePath = saveResult.Response.Data;
                }

                var need = new CharityNeed
                {
                    CharityNeedId = Guid.NewGuid(),
                    CharityId = charity.CharityId,
                    Category = request.Category,
                    ProductName = request.ProductName.Trim(),
                    Quantity = request.Quantity,
                    ProductImage = imagePath,
                    Priority = request.Priority,
                    Status = CharityNeedStatus.Pending,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var created = await _charityNeedRepository.CreateAsync(need);

                return ServiceResult<CharityNeedDetailResponseDTO>
                    .Created("تم إنشاء احتياج الجمعية بنجاح. هو الآن في انتظار موافقة الإدارة.",
                             MapToDetail(created));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in CharityService");
                return ServiceResult<CharityNeedDetailResponseDTO>
                    .Internal("حدث خطأ غير متوقع", new { message = ex.Message });
            }
        }

        /// <inheritdoc/>
        public async Task<ServiceResult<IEnumerable<CharityNeedDetailResponseDTO>>> GetMyCharityNeedsAsync(
            Guid userId,
            MyCharityNeedsFilterDTO query)
        {
            try
            {
                if (query.Page <= 0)
                    return ServiceResult<IEnumerable<CharityNeedDetailResponseDTO>>.InvalidPage();

                if (query.PageSize <= 0)
                    return ServiceResult<IEnumerable<CharityNeedDetailResponseDTO>>.InvalidPageSize();

                if (query.PageSize > MaxPageSize)
                    return ServiceResult<IEnumerable<CharityNeedDetailResponseDTO>>.PageSizeTooLarge();

                var charity = await _profileRepository.GetCharityByUserIdAsync(userId);
                if (charity is null)
                    return ServiceResult<IEnumerable<CharityNeedDetailResponseDTO>>
                        .NotFound("ملف الجمعية غير موجود.");

                var items = await _charityNeedRepository.GetByCharityIdAsync(
                    charity.CharityId, query.Status, query.Page, query.PageSize);

                var totalCount = await _charityNeedRepository.CountByCharityIdAsync(
                    charity.CharityId, query.Status);

                var data = items.Select(MapToDetail);
                var pagination = PaginationInfo.Create(query.Page, query.PageSize, totalCount);

                return ServiceResult<IEnumerable<CharityNeedDetailResponseDTO>>
                    .SuccessPaginated("تم استرجاع احتياجات الجمعية بنجاح", data, pagination);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in CharityService");
                return ServiceResult<IEnumerable<CharityNeedDetailResponseDTO>>
                    .Internal("حدث خطأ غير متوقع", new { message = ex.Message });
            }
        }

        /// <inheritdoc/>
        public async Task<ServiceResult<CharityNeedDetailResponseDTO>> GetMyCharityNeedByIdAsync(
            Guid userId,
            Guid charityNeedId)
        {
            try
            {
                var charity = await _profileRepository.GetCharityByUserIdAsync(userId);
                if (charity is null)
                    return ServiceResult<CharityNeedDetailResponseDTO>
                        .NotFound("ملف الجمعية غير موجود.");

                var need = await _charityNeedRepository.GetByIdWithCharityAsync(charityNeedId);
                if (need is null)
                    return ServiceResult<CharityNeedDetailResponseDTO>
                        .NotFound("احتياج الجمعية غير موجود.");

                if (need.CharityId != charity.CharityId)
                    return ServiceResult<CharityNeedDetailResponseDTO>
                        .Forbidden("ليس لديك الإذن لعرض هذا الاحتياج.");

                return ServiceResult<CharityNeedDetailResponseDTO>
                    .Success("تم استرجاع احتياج الجمعية بنجاح", MapToDetail(need));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in CharityService");
                return ServiceResult<CharityNeedDetailResponseDTO>
                    .Internal("حدث خطأ غير متوقع", new { message = ex.Message });
            }
        }

        /// <inheritdoc/>
        public async Task<ServiceResult<object>> UpdateCharityNeedAsync(
            Guid userId,
            Guid charityNeedId,
            UpdateCharityNeedRequestDTO request)
        {
            try
            {
                var charity = await _profileRepository.GetCharityByUserIdAsync(userId);
                if (charity is null)
                    return ServiceResult<object>.NotFound("ملف الجمعية غير موجود.");

                var need = await _charityNeedRepository.GetByIdWithCharityAsync(charityNeedId);
                if (need is null)
                    return ServiceResult<object>.NotFound("احتياج الجمعية غير موجود.");

                if (need.CharityId != charity.CharityId)
                    return ServiceResult<object>
                        .Forbidden("ليس لديك الإذن لتحديث هذا الاحتياج.");

                if (need.Status != CharityNeedStatus.Pending)
                    return ServiceResult<object>.Error(
                        "Only pending charity needs can be updated.",
                        ErrorCode.INVALID_STATUS,
                        System.Net.HttpStatusCode.UnprocessableEntity);

                // Handle optional image replacement
                if (request.ProductImage is not null)
                {
                    var saveResult = await _fileService
                        .SaveImageAsync(request.ProductImage, ImageFolder.Needs);

                    if (!saveResult.Response.Success)
                        return ServiceResult<object>
                            .BadRequest(saveResult.Response.Message,
                                        saveResult.Response.Error?.Details);

                    // Delete old image silently
                    await _fileService.DeleteImageAsync(need.ProductImage);
                    need.ProductImage = saveResult.Response.Data;
                }

                // Apply only non-null fields
                if (request.Category.HasValue)
                    need.Category = request.Category.Value;

                if (!string.IsNullOrWhiteSpace(request.ProductName))
                    need.ProductName = request.ProductName.Trim();

                if (request.Quantity.HasValue)
                    need.Quantity = request.Quantity.Value;

                if (request.Priority.HasValue)
                    need.Priority = request.Priority.Value;

                need.UpdatedAt = DateTime.UtcNow;

                await _charityNeedRepository.UpdateAsync(need);

                return ServiceResult<object>.Success("تم تحديث احتياج الجمعية بنجاح.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in CharityService");
                return ServiceResult<object>
                    .Internal("حدث خطأ غير متوقع", new { message = ex.Message });
            }
        }

        /// <inheritdoc/>
        public async Task<ServiceResult<object>> DeleteCharityNeedAsync(
            Guid userId,
            Guid charityNeedId)
        {
            try
            {
                var charity = await _profileRepository.GetCharityByUserIdAsync(userId);
                if (charity is null)
                    return ServiceResult<object>.NotFound("ملف الجمعية غير موجود.");

                var need = await _charityNeedRepository.GetByIdWithCharityAsync(charityNeedId);
                if (need is null)
                    return ServiceResult<object>.NotFound("احتياج الجمعية غير موجود.");

                if (need.CharityId != charity.CharityId)
                    return ServiceResult<object>
                        .Forbidden("ليس لديك الإذن لحذف هذا الاحتياج.");

                if (need.Status != CharityNeedStatus.Pending)
                    return ServiceResult<object>.Error(
                        "Only pending charity needs can be deleted.",
                        ErrorCode.INVALID_STATUS,
                        System.Net.HttpStatusCode.UnprocessableEntity);

                // Delete associated image silently before removing the record
                await _fileService.DeleteImageAsync(need.ProductImage);

                await _charityNeedRepository.DeleteAsync(need);

                return ServiceResult<object>.Success("تم حذف احتياج الجمعية بنجاح.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in CharityService");
                return ServiceResult<object>
                    .Internal("حدث خطأ غير متوقع", new { message = ex.Message });
            }
        }

        /// <inheritdoc/>
        public async Task<ServiceResult<object>> FulfillCharityNeedAsync(
            Guid userId,
            Guid charityNeedId)
        {
            try
            {
                var charity = await _profileRepository.GetCharityByUserIdAsync(userId);
                if (charity is null)
                    return ServiceResult<object>.NotFound("ملف الجمعية غير موجود.");

                var need = await _charityNeedRepository.GetByIdWithCharityAsync(charityNeedId);
                if (need is null)
                    return ServiceResult<object>.NotFound("احتياج الجمعية غير موجود.");

                if (need.CharityId != charity.CharityId)
                    return ServiceResult<object>
                        .Forbidden("ليس لديك الإذن لتمييز هذا الاحتياج كمكتمل.");

                if (need.Status != CharityNeedStatus.Approved)
                    return ServiceResult<object>.Error(
                        "Only approved charity needs can be marked as fulfilled.",
                        ErrorCode.INVALID_STATUS,
                        System.Net.HttpStatusCode.UnprocessableEntity);

                need.Status = CharityNeedStatus.Fulfilled;
                need.UpdatedAt = DateTime.UtcNow;

                await _charityNeedRepository.UpdateAsync(need);

                return ServiceResult<object>.Success("تم تمييز احتياج الجمعية كمكتمل.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in CharityService");
                return ServiceResult<object>
                    .Internal("حدث خطأ غير متوقع", new { message = ex.Message });
            }
        }

        // =========================================================
        // NEED APPLICATIONS — received
        // =========================================================

        /// <inheritdoc/>
        public async Task<ServiceResult<IEnumerable<NeedApplicationResponseDTO>>> GetReceivedApplicationsAsync(
            Guid userId,
            PaginationFilterDTO query)
        {
            try
            {
                if (query.Page <= 0)
                    return ServiceResult<IEnumerable<NeedApplicationResponseDTO>>.InvalidPage();

                if (query.PageSize <= 0)
                    return ServiceResult<IEnumerable<NeedApplicationResponseDTO>>.InvalidPageSize();

                if (query.PageSize > MaxPageSize)
                    return ServiceResult<IEnumerable<NeedApplicationResponseDTO>>.PageSizeTooLarge();

                var charity = await _profileRepository.GetCharityByUserIdAsync(userId);
                if (charity is null)
                    return ServiceResult<IEnumerable<NeedApplicationResponseDTO>>
                        .NotFound("ملف الجمعية غير موجود.");

                var items = await _needApplicationRepository.GetReceivedByCharityIdAsync(
                    charity.CharityId, query.Page, query.PageSize);

                var totalCount = await _needApplicationRepository
                    .CountReceivedByCharityIdAsync(charity.CharityId);

                var data = items.Select(na => new NeedApplicationResponseDTO
                {
                    NeedApplicationId = na.NeedApplicationId,
                    CharityNeedId = na.CharityNeedId,
                    ProductName = na.CharityNeed.ProductName,
                    DonorOrganizationId = na.DonorOrganizationId,
                    DonorOrganizationName = na.DonorOrganization.DonorOrganizationName,
                    Status = na.Status,
                    Email = na.DonorOrganization.ApplicationUser.Email,
                    Phone = na.DonorOrganization.ApplicationUser.PhoneNumber,
                    Whatsapp = na.DonorOrganization.ApplicationUser.Whatsapp,
                    Description = na.DonorOrganization.DonorOrganizationDescription,
                    ProductImage = _fileService.GetImageUrl(na.CharityNeed.ProductImage),
                    CreatedAt = na.CreatedAt
                });

                var pagination = PaginationInfo.Create(query.Page, query.PageSize, totalCount);

                return ServiceResult<IEnumerable<NeedApplicationResponseDTO>>
                    .SuccessPaginated("تم استرجاع الطلبات بنجاح", data, pagination);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in CharityService");
                return ServiceResult<IEnumerable<NeedApplicationResponseDTO>>
                    .Internal("حدث خطأ غير متوقع", new { message = ex.Message });
            }
        }

        /// <inheritdoc/>
        public async Task<ServiceResult<object>> AcceptNeedApplicationAsync(
            Guid userId,
            Guid needApplicationId)
        {
            try
            {
                var (application, guardResult) =
                    await GuardNeedApplicationAsync<object>(userId, needApplicationId);

                if (guardResult is not null) return guardResult;

                application!.Status = ApplicationStatus.Accepted;
                application.UpdatedAt = DateTime.UtcNow;

                await _needApplicationRepository.UpdateAsync(application);

                // Notify the donor that their need application was accepted
                await _emailService.SendNeedApplicationAcceptedAsync(
                    application.DonorOrganization.ApplicationUser.Email!,
                    application.DonorOrganization.ApplicationUser.UserName!,
                    application.CharityNeed.ProductName);

                return ServiceResult<object>.Success("تم قبول طلب الاحتياج.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in CharityService");
                return ServiceResult<object>
                    .Internal("حدث خطأ غير متوقع", new { message = ex.Message });
            }
        }

        /// <inheritdoc/>
        public async Task<ServiceResult<object>> RejectNeedApplicationAsync(
            Guid userId,
            Guid needApplicationId)
        {
            try
            {
                var (application, guardResult) =
                    await GuardNeedApplicationAsync<object>(userId, needApplicationId);

                if (guardResult is not null) return guardResult;

                application!.Status = ApplicationStatus.Rejected;
                application.UpdatedAt = DateTime.UtcNow;

                await _needApplicationRepository.UpdateAsync(application);

                // Notify the donor that their need application was rejected
                await _emailService.SendNeedApplicationRejectedAsync(
                    application.DonorOrganization.ApplicationUser.Email!,
                    application.DonorOrganization.ApplicationUser.UserName!,
                    application.CharityNeed.ProductName);

                return ServiceResult<object>.Success("تم رفض طلب الاحتياج.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in CharityService");
                return ServiceResult<object>
                    .Internal("حدث خطأ غير متوقع", new { message = ex.Message });
            }
        }

        // =========================================================
        // OFFER APPLICATIONS — sent
        // =========================================================

        /// <inheritdoc/>
        public async Task<ServiceResult<object>> ApplyToOfferAsync(
            Guid userId,
            Guid offerId)
        {
            try
            {
                var charity = await _profileRepository.GetCharityByUserIdAsync(userId);
                if (charity is null)
                    return ServiceResult<object>.NotFound("ملف الجمعية غير موجود.");

                if (!charity.IsVerified || !charity.IsActive)
                    return ServiceResult<object>
                        .Forbidden("يجب أن يكون حساب الجمعية الخاص بك مفعلاً ونشطاً لتتمكن من التقديم على العروض.");

                var offer = await _offerRepository.GetApprovedOfferByIdAsync(offerId);
                if (offer is null)
                    return ServiceResult<object>
                        .NotFound("العرض غير موجود أو لم يعد متاحاً.");

                var alreadyApplied = await _offerApplicationRepository
                    .ExistsAsync(charity.CharityId, offerId);

                if (alreadyApplied)
                    return ServiceResult<object>
                        .Conflict("لقد قمت بالتقديم على هذا العرض بالفعل.");

                var application = new OfferApplication
                {
                    OfferApplicationId = Guid.NewGuid(),
                    OfferId = offerId,
                    CharityId = charity.CharityId,
                    Status = ApplicationStatus.Pending,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _offerApplicationRepository.CreateAsync(application);

                // Notify the donor that a charity has applied to their offer
                await _emailService.SendOfferApplicationReceivedAsync(
                    offer.DonorOrganization.ApplicationUser.Email!,
                    offer.DonorOrganization.ApplicationUser.UserName!,
                    charity.CharityName,
                    offer.ProductName);

                return ServiceResult<object>
                    .Created("تم تقديم الطلب بنجاح.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in CharityService");
                return ServiceResult<object>
                    .Internal("حدث خطأ غير متوقع", new { message = ex.Message });
            }
        }

        /// <inheritdoc/>
        public async Task<ServiceResult<IEnumerable<MyOfferApplicationResponseDTO>>> GetSentApplicationsAsync(
            Guid userId,
            PaginationFilterDTO query)
        {
            try
            {
                if (query.Page <= 0)
                    return ServiceResult<IEnumerable<MyOfferApplicationResponseDTO>>.InvalidPage();

                if (query.PageSize <= 0)
                    return ServiceResult<IEnumerable<MyOfferApplicationResponseDTO>>.InvalidPageSize();

                if (query.PageSize > MaxPageSize)
                    return ServiceResult<IEnumerable<MyOfferApplicationResponseDTO>>.PageSizeTooLarge();

                var charity = await _profileRepository.GetCharityByUserIdAsync(userId);
                if (charity is null)
                    return ServiceResult<IEnumerable<MyOfferApplicationResponseDTO>>
                        .NotFound("ملف الجمعية غير موجود.");

                var items = await _offerApplicationRepository.GetSentByCharityIdAsync(
                    charity.CharityId, query.Page, query.PageSize);

                var totalCount = await _offerApplicationRepository
                    .CountSentByCharityIdAsync(charity.CharityId);

                var data = items.Select(oa => new MyOfferApplicationResponseDTO
                {
                    OfferApplicationId = oa.OfferApplicationId,
                    OfferId = oa.OfferId,
                    ProductName = oa.Offer.ProductName,
                    DonorOrganizationName = oa.Offer.DonorOrganization.DonorOrganizationName,
                    CharityName = oa.Charity.CharityName,
                    Status = oa.Status,
                    Email = oa.Offer.DonorOrganization.ApplicationUser.Email,
                    Phone = oa.Offer.DonorOrganization.ApplicationUser.PhoneNumber,
                    Whatsapp = oa.Offer.DonorOrganization.ApplicationUser.Whatsapp,
                    Description = oa.Offer.DonorOrganization.DonorOrganizationDescription,
                    ProductImage = _fileService.GetImageUrl(oa.Offer.ProductImage),
                    CreatedAt = oa.CreatedAt
                });

                var pagination = PaginationInfo.Create(query.Page, query.PageSize, totalCount);

                return ServiceResult<IEnumerable<MyOfferApplicationResponseDTO>>
                    .SuccessPaginated("تم استرجاع الطلبات بنجاح", data, pagination);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in CharityService");
                return ServiceResult<IEnumerable<MyOfferApplicationResponseDTO>>
                    .Internal("حدث خطأ غير متوقع", new { message = ex.Message });
            }
        }

        /// <inheritdoc/>
        public async Task<ServiceResult<object>> CancelOfferApplicationAsync(
            Guid userId,
            Guid offerApplicationId)
        {
            try
            {
                var charity = await _profileRepository.GetCharityByUserIdAsync(userId);
                if (charity is null)
                    return ServiceResult<object>.NotFound("ملف الجمعية غير موجود.");

                var application = await _offerApplicationRepository
                    .GetByIdAsync(offerApplicationId);

                if (application is null)
                    return ServiceResult<object>.NotFound("طلب العرض غير موجود.");

                if (application.CharityId != charity.CharityId)
                    return ServiceResult<object>
                        .Forbidden("ليس لديك الإذن لإلغاء هذا الطلب.");

                if (application.Status != ApplicationStatus.Pending)
                    return ServiceResult<object>.Error(
                        "Only pending offer applications can be cancelled.",
                        ErrorCode.INVALID_STATUS,
                        System.Net.HttpStatusCode.UnprocessableEntity);

                await _offerApplicationRepository.DeleteAsync(application);

                return ServiceResult<object>.Success("تم إلغاء طلب العرض.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in CharityService");
                return ServiceResult<object>
                    .Internal("حدث خطأ غير متوقع", new { message = ex.Message });
            }
        }

        // =========================================================
        // PRIVATE HELPERS
        // =========================================================

        /// <summary>
        /// Shared guard for AcceptNeedApplication and RejectNeedApplication.
        /// Validates ownership and Pending status.
        /// Returns (application, null) on success.
        /// Returns (null, errorResult) on failure.
        /// </summary>
        private async Task<(NeedApplication? Application, ServiceResult<T>? Error)>
            GuardNeedApplicationAsync<T>(Guid userId, Guid needApplicationId)
        {
            var charity = await _profileRepository.GetCharityByUserIdAsync(userId);
            if (charity is null)
                return (null, ServiceResult<T>.NotFound("Charity profile not found."));

            var application = await _needApplicationRepository.GetByIdAsync(needApplicationId);
            if (application is null)
                return (null, ServiceResult<T>.NotFound("طلب الاحتياج غير موجود."));

            if (application.CharityNeed.CharityId != charity.CharityId)
                return (null, ServiceResult<T>.Forbidden(
                    "ليس لديك الإذن للرد على هذا الطلب."));

            if (application.Status != ApplicationStatus.Pending)
                return (null, ServiceResult<T>.Error(
                    "Only pending need applications can be accepted or rejected.",
                    ErrorCode.INVALID_STATUS,
                    System.Net.HttpStatusCode.UnprocessableEntity));

            return (application, null);
        }

        /// <summary>
        /// Maps a <see cref="CharityNeed"/> entity to a <see cref="CharityNeedDetailResponseDTO"/>.
        /// Builds the full image URL if a relative path is stored.
        /// </summary>
        private CharityNeedDetailResponseDTO MapToDetail(CharityNeed need)
            => new()
            {
                CharityNeedId = need.CharityNeedId,
                ProductName = need.ProductName,
                Category = need.Category,
                Quantity = need.Quantity,
                ProductImage = _fileService.GetImageUrl(need.ProductImage),
                Priority = need.Priority,
                Status = need.Status,
                CreatedAt = need.CreatedAt,
                UpdatedAt = need.UpdatedAt
            };
    }
}