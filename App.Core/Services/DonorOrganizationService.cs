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
    public class DonorOrganizationService : IDonorOrganizationService
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IOfferRepository _offerRepository;
        private readonly IOfferApplicationRepository _offerApplicationRepository;
        private readonly INeedApplicationRepository _needApplicationRepository;
        private readonly ICharityNeedRepository _charityNeedRepository;
        private readonly IFileService _fileService;
        private readonly IEmailService _emailService;
        private readonly ILogger<DonorOrganizationService> _logger;

        public DonorOrganizationService(
            IProfileRepository profileRepository,
            IOfferRepository offerRepository,
            IOfferApplicationRepository offerApplicationRepository,
            INeedApplicationRepository needApplicationRepository,
            ICharityNeedRepository charityNeedRepository,
            IFileService fileService,
            IEmailService emailService,
            ILogger<DonorOrganizationService> logger)
        {
            _profileRepository = profileRepository;
            _offerRepository = offerRepository;
            _offerApplicationRepository = offerApplicationRepository;
            _needApplicationRepository = needApplicationRepository;
            _charityNeedRepository = charityNeedRepository;
            _fileService = fileService;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<ServiceResult<DonorDashboardResponseDTO>> GetDashboardAsync(Guid userId)
        {
            try
            {
                var Donor = await _profileRepository.GetDonorOrganizationByUserIdAsync(userId);
                if (Donor is null)
                {
                    return ServiceResult<DonorDashboardResponseDTO>.NotFound("ملف المؤسسة المانحة غير موجود.");
                }
                var offerCounts = await _offerRepository
                    .GetOfferCountsByDonorOrganizationIdAsync(Donor.DonorOrganizationId);

                var receivedCounts = await _offerApplicationRepository
                    .GetReceivedCountsByDonorOrganizationIdAsync(Donor.DonorOrganizationId);

                var sentCounts = await _needApplicationRepository
                    .GetSentCountsByDonorOrganizationIdAsync(Donor.DonorOrganizationId);
                var data = new DonorDashboardResponseDTO
                {
                    //offer counts
                    TotalOffers = offerCounts.Total,
                    PendingOffers = offerCounts.Pending,
                    ApprovedOffers = offerCounts.Approved,
                    RejectedOffers = offerCounts.Rejected,
                    FulfilledOffers = offerCounts.Fulfilled,
                    ExpiredOffers = offerCounts.Expired,
                    //offer applications received
                    TotalOfferApplicationsReceived = receivedCounts.Total,
                    PendingOfferApplicationsReceived = receivedCounts.Pending,
                    AcceptedOfferApplicationsReceived = receivedCounts.Accepted,
                    RejectedOfferApplicationsReceived = receivedCounts.Rejected,

                    //need applications sent
                    TotalNeedApplicationsSent = sentCounts.Total,
                    PendingNeedApplicationsSent = sentCounts.Pending,
                    AcceptedNeedApplicationsSent = sentCounts.Accepted,
                    RejectedNeedApplicationsSent = sentCounts.Rejected
                };
                return ServiceResult<DonorDashboardResponseDTO>
                   .Success("تم استرجاع إحصائيات لوحة التحكم بنجاح", data);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in DonorOrganizationService");
                return ServiceResult<DonorDashboardResponseDTO>
                  .Internal("حدث خطأ غير متوقع", new { message = ex.Message });
            }
        }
        public async Task<ServiceResult<OfferDetailResponseDTO>> CreateOfferAsync(Guid userId, CreateOfferRequestDTO request)
        {
            try
            {
                var Donor = await _profileRepository.GetDonorOrganizationByUserIdAsync(userId);
                if (Donor is null)
                {
                    return ServiceResult<OfferDetailResponseDTO>.NotFound("ملف المؤسسة المانحة غير موجود.");
                }
                if (!Donor.IsVerified || !Donor.IsActive)
                {
                    return ServiceResult<OfferDetailResponseDTO>.Forbidden("يجب أن يكون حساب المتبرع الخاص بك مفعلاً ونشطاً لتتمكن من نشر العروض.");
                }
                // Handle optional image upload
                string? imagePath = null;
                if (request.ProductImage is not null)
                {
                    var saveResult = await _fileService.SaveImageAsync(request.ProductImage, ImageFolder.Offers);

                    if (!saveResult.Response.Success)
                        return ServiceResult<OfferDetailResponseDTO>
                            .BadRequest(saveResult.Response.Message,
                                        saveResult.Response.Error?.Details);

                    imagePath = saveResult.Response.Data;
                }
                var offer = new Offer
                {
                    DonorOrganizationId = Donor.DonorOrganizationId,
                    Category = request.Category,
                    ProductName = request.ProductName,
                    Quantity = request.Quantity,
                    ExpiryDate = request.ExpiryDate,
                    ProductImage = imagePath,
                    Status = OfferStatus.Pending,
                    Description = request.Description,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                var created = await _offerRepository.CreateAsync(offer);
                return ServiceResult<OfferDetailResponseDTO>
                    .Created("تم إنشاء العرض بنجاح. هو الآن في انتظار موافقة الإدارة.",
                             MapToOfferDetailDTO(created));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in DonorOrganizationService");
                return ServiceResult<OfferDetailResponseDTO>
                  .Internal("حدث خطأ غير متوقع", new { message = ex.Message });
            }
        }

        public async Task<ServiceResult<IEnumerable<OfferDetailResponseDTO>>> GetMyOffersAsync(Guid userId, MyOffersFilterDTO query)
        {
            try
            {
                if (query.Page <= 0) return ServiceResult<IEnumerable<OfferDetailResponseDTO>>.InvalidPage();
                if (query.PageSize <= 0) return ServiceResult<IEnumerable<OfferDetailResponseDTO>>.InvalidPageSize();

                var donor = await _profileRepository.GetDonorOrganizationByUserIdAsync(userId);
                if (donor is null) return ServiceResult<IEnumerable<OfferDetailResponseDTO>>.NotFound("المتبرع غير موجود.");

                var items = await _offerRepository.GetByDonorOrganizationIdAsync(
                    donor.DonorOrganizationId, query.Status, query.Page, query.PageSize);
                var totalCount = await _offerRepository.CountByDonorOrganizationIdAsync(
                    donor.DonorOrganizationId, query.Status);

                var data = items.Select(MapToOfferDetailDTO);
                var pagination = PaginationInfo.Create(query.Page, query.PageSize, totalCount);

                return ServiceResult<IEnumerable<OfferDetailResponseDTO>>
                    .SuccessPaginated("تم استرجاع العروض بنجاح", data, pagination);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in DonorOrganizationService");
                return ServiceResult<IEnumerable<OfferDetailResponseDTO>>.Internal("Error", new { message = ex.Message });
            }
        }

        public async Task<ServiceResult<OfferDetailResponseDTO>> GetMyOfferByIdAsync(Guid userId, Guid offerId)
        {
            try
            {
                var donor = await _profileRepository.GetDonorOrganizationByUserIdAsync(userId);
                if (donor is null) return ServiceResult<OfferDetailResponseDTO>.NotFound("المتبرع غير موجود.");

                var offer = await _offerRepository.GetByIdWithDonorAsync(offerId);
                if (offer is null || offer.DonorOrganizationId != donor.DonorOrganizationId)
                    return ServiceResult<OfferDetailResponseDTO>.NotFound("العرض غير موجود.");

                return ServiceResult<OfferDetailResponseDTO>.Success("تم استرجاع العرض", MapToOfferDetailDTO(offer));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in DonorOrganizationService");
                return ServiceResult<OfferDetailResponseDTO>.Internal("Error", new { message = ex.Message });
            }
        }

        public async Task<ServiceResult<object>> UpdateOfferAsync(Guid userId, Guid offerId, UpdateOfferRequestDTO request)
        {
            try
            {
                var donor = await _profileRepository.GetDonorOrganizationByUserIdAsync(userId);
                if (donor is null) return ServiceResult<object>.NotFound("المتبرع غير موجود.");

                var offer = await _offerRepository.GetByIdWithDonorAsync(offerId);
                if (offer is null || offer.DonorOrganizationId != donor.DonorOrganizationId)
                    return ServiceResult<object>.NotFound("العرض غير موجود.");

                if (offer.Status != OfferStatus.Pending)
                    return ServiceResult<object>.Error("Only pending offers can be updated.", ErrorCode.INVALID_STATUS, System.Net.HttpStatusCode.UnprocessableEntity);

                if (request.ProductImage is not null)
                {
                    var saveResult = await _fileService.SaveImageAsync(request.ProductImage, ImageFolder.Offers);
                    if (!saveResult.Response.Success) return ServiceResult<object>.BadRequest(saveResult.Response.Message);
                    await _fileService.DeleteImageAsync(offer.ProductImage);
                    offer.ProductImage = saveResult.Response.Data;
                }

                if (request.Category.HasValue) offer.Category = request.Category.Value;
                if (!string.IsNullOrWhiteSpace(request.ProductName)) offer.ProductName = request.ProductName.Trim();
                if (request.Quantity.HasValue) offer.Quantity = request.Quantity.Value;
                if (request.ExpiryDate.HasValue) offer.ExpiryDate = request.ExpiryDate.Value;
                if (request.Description is not null) offer.Description = request.Description;

                offer.UpdatedAt = DateTime.UtcNow;
                await _offerRepository.UpdateAsync(offer);

                return ServiceResult<object>.Success("تم تحديث العرض بنجاح.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in DonorOrganizationService");
                return ServiceResult<object>.Internal("Error", new { message = ex.Message });
            }
        }

        public async Task<ServiceResult<object>> DeleteOfferAsync(Guid userId, Guid offerId)
        {
            try
            {
                var donor = await _profileRepository.GetDonorOrganizationByUserIdAsync(userId);
                if (donor is null) return ServiceResult<object>.NotFound("المتبرع غير موجود.");

                var offer = await _offerRepository.GetByIdWithDonorAsync(offerId);
                if (offer is null || offer.DonorOrganizationId != donor.DonorOrganizationId)
                    return ServiceResult<object>.NotFound("العرض غير موجود.");

                if (offer.Status != OfferStatus.Pending)
                    return ServiceResult<object>.Error("Only pending offers can be deleted.", ErrorCode.INVALID_STATUS, System.Net.HttpStatusCode.UnprocessableEntity);

                await _fileService.DeleteImageAsync(offer.ProductImage);
                await _offerRepository.DeleteAsync(offer);

                return ServiceResult<object>.Success("تم حذف العرض بنجاح.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in DonorOrganizationService");
                return ServiceResult<object>.Internal("Error", new { message = ex.Message });
            }
        }

        public async Task<ServiceResult<object>> FulfillOfferAsync(Guid userId, Guid offerId)
        {
            try
            {
                var donor = await _profileRepository.GetDonorOrganizationByUserIdAsync(userId);
                if (donor is null) return ServiceResult<object>.NotFound("المتبرع غير موجود.");

                var offer = await _offerRepository.GetByIdWithDonorAsync(offerId);
                if (offer is null || offer.DonorOrganizationId != donor.DonorOrganizationId)
                    return ServiceResult<object>.NotFound("العرض غير موجود.");

                if (offer.Status != OfferStatus.Approved)
                    return ServiceResult<object>.Error("Only approved offers can be fulfilled.", ErrorCode.INVALID_STATUS, System.Net.HttpStatusCode.UnprocessableEntity);

                offer.Status = OfferStatus.Fulfilled;
                offer.UpdatedAt = DateTime.UtcNow;
                await _offerRepository.UpdateAsync(offer);

                return ServiceResult<object>.Success("تم إكمال العرض.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in DonorOrganizationService");
                return ServiceResult<object>.Internal("Error", new { message = ex.Message });
            }
        }

        public async Task<ServiceResult<IEnumerable<MyOfferApplicationResponseDTO>>> GetReceivedApplicationsAsync(Guid userId, PaginationFilterDTO query)
        {
            try
            {
                if (query.Page <= 0) return ServiceResult<IEnumerable<MyOfferApplicationResponseDTO>>.InvalidPage();
                if (query.PageSize <= 0) return ServiceResult<IEnumerable<MyOfferApplicationResponseDTO>>.InvalidPageSize();

                var donor = await _profileRepository.GetDonorOrganizationByUserIdAsync(userId);
                if (donor is null) return ServiceResult<IEnumerable<MyOfferApplicationResponseDTO>>.NotFound("المتبرع غير موجود.");

                var items = await _offerApplicationRepository.GetReceivedByDonorOrganizationIdAsync(
                    donor.DonorOrganizationId, query.Page, query.PageSize);

                var counts = await _offerApplicationRepository.GetReceivedCountsByDonorOrganizationIdAsync(donor.DonorOrganizationId);

                var data = items.Select(oa => new MyOfferApplicationResponseDTO
                {
                    OfferApplicationId = oa.OfferApplicationId,
                    OfferId = oa.OfferId,
                    ProductName = oa.Offer.ProductName,
                    CharityName = oa.Charity.CharityName,
                    DonorOrganizationName = oa.Offer.DonorOrganization.DonorOrganizationName,
                    Status = oa.Status,
                    Email = oa.Charity.ApplicationUser.Email,
                    Phone = oa.Charity.ApplicationUser.PhoneNumber,
                    Whatsapp = oa.Charity.ApplicationUser.Whatsapp,
                    CharityDescription = oa.Charity.CharityDescription,
                    ProductImage = _fileService.GetImageUrl(oa.Offer.ProductImage),
                    CreatedAt = oa.CreatedAt
                });

                var pagination = PaginationInfo.Create(query.Page, query.PageSize, counts.Total);

                return ServiceResult<IEnumerable<MyOfferApplicationResponseDTO>>
                    .SuccessPaginated("تم استرجاع الطلبات بنجاح", data, pagination);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in DonorOrganizationService");
                return ServiceResult<IEnumerable<MyOfferApplicationResponseDTO>>.Internal("Error", new { message = ex.Message });
            }
        }

        public async Task<ServiceResult<IEnumerable<MyNeedApplicationResponseDTO>>> GetSentApplicationsAsync(Guid userId, PaginationFilterDTO query)
        {
            try
            {
                if (query.Page <= 0) return ServiceResult<IEnumerable<MyNeedApplicationResponseDTO>>.InvalidPage();
                if (query.PageSize <= 0) return ServiceResult<IEnumerable<MyNeedApplicationResponseDTO>>.InvalidPageSize();

                var donor = await _profileRepository.GetDonorOrganizationByUserIdAsync(userId);
                if (donor is null) return ServiceResult<IEnumerable<MyNeedApplicationResponseDTO>>.NotFound("المتبرع غير موجود.");

                var items = await _needApplicationRepository.GetSentByDonorOrganizationIdAsync(
                    donor.DonorOrganizationId, query.Page, query.PageSize);

                var totalCount = await _needApplicationRepository.CountSentByDonorOrganizationIdAsync(donor.DonorOrganizationId);

                var data = items.Select(na => new MyNeedApplicationResponseDTO
                {
                    NeedApplicationId = na.NeedApplicationId,
                    CharityNeedId = na.CharityNeedId,
                    ProductName = na.CharityNeed?.ProductName ?? string.Empty,
                    CharityName = na.CharityNeed?.Charity?.CharityName ?? string.Empty,
                    Status = na.Status,
                    Email = na.CharityNeed?.Charity?.ApplicationUser?.Email,
                    Phone = na.CharityNeed?.Charity?.ApplicationUser?.PhoneNumber,
                    Whatsapp = na.CharityNeed?.Charity?.ApplicationUser?.Whatsapp,
                    CharityDescription = na.CharityNeed?.Charity?.CharityDescription,
                    ProductImage = _fileService.GetImageUrl(na.CharityNeed?.ProductImage),
                    CreatedAt = na.CreatedAt
                });

                var pagination = PaginationInfo.Create(query.Page, query.PageSize, totalCount);

                return ServiceResult<IEnumerable<MyNeedApplicationResponseDTO>>
                    .SuccessPaginated("تم استرجاع الطلبات المرسلة بنجاح", data, pagination);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in DonorOrganizationService");
                return ServiceResult<IEnumerable<MyNeedApplicationResponseDTO>>.Internal("Error", new { message = ex.Message });
            }
        }

        public async Task<ServiceResult<object>> ApplyToCharityNeedAsync(Guid userId, Guid charityNeedId)
        {
            try
            {
                var donor = await _profileRepository.GetDonorOrganizationByUserIdAsync(userId);
                if (donor is null)
                    return ServiceResult<object>.NotFound("ملف المتبرع غير موجود.");

                if (!donor.IsVerified || !donor.IsActive)
                    return ServiceResult<object>
                        .Forbidden("يجب أن يكون حساب المتبرع الخاص بك مفعلاً ونشطاً لتتمكن من التقديم على احتياجات الجمعيات.");

                var charityNeed = await _charityNeedRepository.GetByIdWithCharityAsync(charityNeedId);
                if (charityNeed is null || charityNeed.Status != CharityNeedStatus.Approved)
                    return ServiceResult<object>
                        .NotFound("احتياج الجمعية غير موجود أو لم يعد متاحاً.");

                var alreadyApplied = await _needApplicationRepository
                    .ExistsAsync(donor.DonorOrganizationId, charityNeedId);

                if (alreadyApplied)
                    return ServiceResult<object>
                        .Conflict("لقد قمت بالتقديم على هذا الاحتياج بالفعل.");

                var application = new NeedApplication
                {
                    NeedApplicationId = Guid.NewGuid(),
                    CharityNeedId = charityNeedId,
                    DonorOrganizationId = donor.DonorOrganizationId,
                    Status = ApplicationStatus.Pending,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _needApplicationRepository.CreateAsync(application);

                // Notify the charity that a donor has applied to their need
                await _emailService.SendNeedApplicationReceivedAsync(
                    charityNeed.Charity.ApplicationUser.Email!,
                    charityNeed.Charity.ApplicationUser.UserName!,
                    donor.DonorOrganizationName,
                    charityNeed.ProductName);

                return ServiceResult<object>
                    .Created("تم تقديم الطلب بنجاح.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in DonorOrganizationService");
                return ServiceResult<object>
                    .Internal("حدث خطأ غير متوقع", new { message = ex.Message });
            }
        }

        public async Task<ServiceResult<object>> AcceptOfferApplicationAsync(Guid userId, Guid offerApplicationId)
        {
            try
            {
                var donor = await _profileRepository.GetDonorOrganizationByUserIdAsync(userId);
                if (donor is null) return ServiceResult<object>.NotFound("المتبرع غير موجود.");

                var application = await _offerApplicationRepository.GetByIdAsync(offerApplicationId);
                if (application is null || application.Offer.DonorOrganizationId != donor.DonorOrganizationId)
                    return ServiceResult<object>.NotFound("الطلب غير موجود.");

                if (application.Status != ApplicationStatus.Pending)
                    return ServiceResult<object>.Error("Only pending applications can be accepted.", ErrorCode.INVALID_STATUS, System.Net.HttpStatusCode.UnprocessableEntity);

                application.Status = ApplicationStatus.Accepted;
                application.UpdatedAt = DateTime.UtcNow;
                await _offerApplicationRepository.UpdateAsync(application);

                // Notify the charity that their offer application was accepted
                await _emailService.SendOfferApplicationAcceptedAsync(
                    application.Charity.ApplicationUser.Email!,
                    application.Charity.ApplicationUser.UserName!,
                    application.Offer.ProductName);

                return ServiceResult<object>.Success("تم قبول الطلب.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in DonorOrganizationService");
                return ServiceResult<object>.Internal("Error", new { message = ex.Message });
            }
        }

        public async Task<ServiceResult<object>> RejectOfferApplicationAsync(Guid userId, Guid offerApplicationId)
        {
            try
            {
                var donor = await _profileRepository.GetDonorOrganizationByUserIdAsync(userId);
                if (donor is null) return ServiceResult<object>.NotFound("المتبرع غير موجود.");

                var application = await _offerApplicationRepository.GetByIdAsync(offerApplicationId);
                if (application is null || application.Offer.DonorOrganizationId != donor.DonorOrganizationId)
                    return ServiceResult<object>.NotFound("الطلب غير موجود.");

                if (application.Status != ApplicationStatus.Pending)
                    return ServiceResult<object>.Error("Only pending applications can be rejected.", ErrorCode.INVALID_STATUS, System.Net.HttpStatusCode.UnprocessableEntity);

                application.Status = ApplicationStatus.Rejected;
                application.UpdatedAt = DateTime.UtcNow;
                await _offerApplicationRepository.UpdateAsync(application);

                // Notify the charity that their offer application was rejected
                await _emailService.SendOfferApplicationRejectedAsync(
                    application.Charity.ApplicationUser.Email!,
                    application.Charity.ApplicationUser.UserName!,
                    application.Offer.ProductName);

                return ServiceResult<object>.Success("تم رفض الطلب.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in DonorOrganizationService");
                return ServiceResult<object>.Internal("Error", new { message = ex.Message });
            }
        }

        // Private Helpers
        private OfferDetailResponseDTO MapToOfferDetailDTO(Offer offer)
        {
            return new OfferDetailResponseDTO
            {
                OfferId = offer.OfferId,
                ProductName = offer.ProductName,
                Category = offer.Category,
                Quantity = offer.Quantity,
                ProductImage = _fileService.GetImageUrl(offer.ProductImage),
                ExpiryDate = offer.ExpiryDate,
                Status = offer.Status,
                CreatedAt = offer.CreatedAt,
                UpdatedAt = offer.UpdatedAt,
                Description = offer.Description,
                DonorOraganizationDesctption = offer.DonorOrganization?.DonorOrganizationDescription,
                Email = offer.DonorOrganization?.ApplicationUser?.Email,
                Phone = offer.DonorOrganization?.ApplicationUser?.PhoneNumber,
                Whatsapp = offer.DonorOrganization?.ApplicationUser?.Whatsapp
            };
        }
    }
}
