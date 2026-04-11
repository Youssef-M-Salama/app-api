using App.Core.Domain.Entities;
using App.Core.Domain.RepositoryContracts;
using App.Core.DTOs.Request;
using App.Core.DTOs.Response;
using App.Core.DTOs.ResultPattern;
using App.Core.Enums;
using App.Core.ServiceContracts;

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

        private const int MaxPageSize = 50;

        public CharityService(
            ICharityRepository charityRepository,
            ICharityNeedRepository charityNeedRepository,
            INeedApplicationRepository needApplicationRepository,
            IOfferApplicationRepository offerApplicationRepository,
            IOfferRepository offerRepository,
            IProfileRepository profileRepository,
            IFileService fileService)
        {
            _charityRepository = charityRepository;
            _charityNeedRepository = charityNeedRepository;
            _needApplicationRepository = needApplicationRepository;
            _offerApplicationRepository = offerApplicationRepository;
            _offerRepository = offerRepository;
            _profileRepository = profileRepository;
            _fileService = fileService;
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
                        .NotFound("Charity profile not found.");

                // Run all 3 count queries concurrently — single round trip each
                var needCountsTask = _charityNeedRepository
                    .GetNeedCountsByCharityIdAsync(charity.CharityId);

                var receivedCountsTask = _needApplicationRepository
                    .GetReceivedCountsByCharityIdAsync(charity.CharityId);

                var sentCountsTask = _offerApplicationRepository
                    .GetSentCountsByCharityIdAsync(charity.CharityId);

                await Task.WhenAll(needCountsTask, receivedCountsTask, sentCountsTask);

                var needCounts = needCountsTask.Result;
                var receivedCounts = receivedCountsTask.Result;
                var sentCounts = sentCountsTask.Result;

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
                    .Success("Dashboard statistics retrieved successfully", data);
            }
            catch (Exception ex)
            {
                return ServiceResult<CharityDashboardResponseDTO>
                    .Internal("An unexpected error occurred", new { message = ex.Message });
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
                        .NotFound("Charity profile not found.");

                if (!charity.IsVerified || !charity.IsActive)
                    return ServiceResult<CharityNeedDetailResponseDTO>
                        .Forbidden("Your charity account must be verified and active to post needs.");

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
                    .Created("Charity need created successfully. It is now pending admin approval.",
                             MapToDetail(created));
            }
            catch (Exception ex)
            {
                return ServiceResult<CharityNeedDetailResponseDTO>
                    .Internal("An unexpected error occurred", new { message = ex.Message });
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
                        .NotFound("Charity profile not found.");

                var items = await _charityNeedRepository.GetByCharityIdAsync(
                    charity.CharityId, query.Status, query.Page, query.PageSize);

                var totalCount = await _charityNeedRepository.CountByCharityIdAsync(
                    charity.CharityId, query.Status);

                var data = items.Select(MapToDetail);
                var pagination = PaginationInfo.Create(query.Page, query.PageSize, totalCount);

                return ServiceResult<IEnumerable<CharityNeedDetailResponseDTO>>
                    .SuccessPaginated("Charity needs retrieved successfully", data, pagination);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<CharityNeedDetailResponseDTO>>
                    .Internal("An unexpected error occurred", new { message = ex.Message });
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
                        .NotFound("Charity profile not found.");

                var need = await _charityNeedRepository.GetByIdWithCharityAsync(charityNeedId);
                if (need is null)
                    return ServiceResult<CharityNeedDetailResponseDTO>
                        .NotFound("Charity need not found.");

                if (need.CharityId != charity.CharityId)
                    return ServiceResult<CharityNeedDetailResponseDTO>
                        .Forbidden("You do not have permission to view this charity need.");

                return ServiceResult<CharityNeedDetailResponseDTO>
                    .Success("Charity need retrieved successfully", MapToDetail(need));
            }
            catch (Exception ex)
            {
                return ServiceResult<CharityNeedDetailResponseDTO>
                    .Internal("An unexpected error occurred", new { message = ex.Message });
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
                    return ServiceResult<object>.NotFound("Charity profile not found.");

                var need = await _charityNeedRepository.GetByIdWithCharityAsync(charityNeedId);
                if (need is null)
                    return ServiceResult<object>.NotFound("Charity need not found.");

                if (need.CharityId != charity.CharityId)
                    return ServiceResult<object>
                        .Forbidden("You do not have permission to update this charity need.");

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

                return ServiceResult<object>.Success("Charity need updated successfully.");
            }
            catch (Exception ex)
            {
                return ServiceResult<object>
                    .Internal("An unexpected error occurred", new { message = ex.Message });
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
                    return ServiceResult<object>.NotFound("Charity profile not found.");

                var need = await _charityNeedRepository.GetByIdWithCharityAsync(charityNeedId);
                if (need is null)
                    return ServiceResult<object>.NotFound("Charity need not found.");

                if (need.CharityId != charity.CharityId)
                    return ServiceResult<object>
                        .Forbidden("You do not have permission to delete this charity need.");

                if (need.Status != CharityNeedStatus.Pending)
                    return ServiceResult<object>.Error(
                        "Only pending charity needs can be deleted.",
                        ErrorCode.INVALID_STATUS,
                        System.Net.HttpStatusCode.UnprocessableEntity);

                // Delete associated image silently before removing the record
                await _fileService.DeleteImageAsync(need.ProductImage);

                await _charityNeedRepository.DeleteAsync(need);

                return ServiceResult<object>.Success("Charity need deleted successfully.");
            }
            catch (Exception ex)
            {
                return ServiceResult<object>
                    .Internal("An unexpected error occurred", new { message = ex.Message });
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
                    return ServiceResult<object>.NotFound("Charity profile not found.");

                var need = await _charityNeedRepository.GetByIdWithCharityAsync(charityNeedId);
                if (need is null)
                    return ServiceResult<object>.NotFound("Charity need not found.");

                if (need.CharityId != charity.CharityId)
                    return ServiceResult<object>
                        .Forbidden("You do not have permission to fulfill this charity need.");

                if (need.Status != CharityNeedStatus.Approved)
                    return ServiceResult<object>.Error(
                        "Only approved charity needs can be marked as fulfilled.",
                        ErrorCode.INVALID_STATUS,
                        System.Net.HttpStatusCode.UnprocessableEntity);

                need.Status = CharityNeedStatus.Fulfilled;
                need.UpdatedAt = DateTime.UtcNow;

                await _charityNeedRepository.UpdateAsync(need);

                return ServiceResult<object>.Success("Charity need marked as fulfilled.");
            }
            catch (Exception ex)
            {
                return ServiceResult<object>
                    .Internal("An unexpected error occurred", new { message = ex.Message });
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
                        .NotFound("Charity profile not found.");

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
                    CreatedAt = na.CreatedAt
                });

                var pagination = PaginationInfo.Create(query.Page, query.PageSize, totalCount);

                return ServiceResult<IEnumerable<NeedApplicationResponseDTO>>
                    .SuccessPaginated("Applications retrieved successfully", data, pagination);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<NeedApplicationResponseDTO>>
                    .Internal("An unexpected error occurred", new { message = ex.Message });
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

                return ServiceResult<object>.Success("Need application accepted.");
            }
            catch (Exception ex)
            {
                return ServiceResult<object>
                    .Internal("An unexpected error occurred", new { message = ex.Message });
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

                return ServiceResult<object>.Success("Need application rejected.");
            }
            catch (Exception ex)
            {
                return ServiceResult<object>
                    .Internal("An unexpected error occurred", new { message = ex.Message });
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
                    return ServiceResult<object>.NotFound("Charity profile not found.");

                if (!charity.IsVerified || !charity.IsActive)
                    return ServiceResult<object>
                        .Forbidden("Your charity account must be verified and active to apply to offers.");

                var offer = await _offerRepository.GetApprovedOfferByIdAsync(offerId);
                if (offer is null)
                    return ServiceResult<object>
                        .NotFound("Offer not found or is no longer available.");

                var alreadyApplied = await _offerApplicationRepository
                    .ExistsAsync(charity.CharityId, offerId);

                if (alreadyApplied)
                    return ServiceResult<object>
                        .Conflict("You have already applied to this offer.");

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

                return ServiceResult<object>
                    .Created("Application submitted successfully.");
            }
            catch (Exception ex)
            {
                return ServiceResult<object>
                    .Internal("An unexpected error occurred", new { message = ex.Message });
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
                        .NotFound("Charity profile not found.");

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
                    Status = oa.Status,
                    CreatedAt = oa.CreatedAt
                });

                var pagination = PaginationInfo.Create(query.Page, query.PageSize, totalCount);

                return ServiceResult<IEnumerable<MyOfferApplicationResponseDTO>>
                    .SuccessPaginated("Applications retrieved successfully", data, pagination);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<MyOfferApplicationResponseDTO>>
                    .Internal("An unexpected error occurred", new { message = ex.Message });
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
                    return ServiceResult<object>.NotFound("Charity profile not found.");

                var application = await _offerApplicationRepository
                    .GetByIdAsync(offerApplicationId);

                if (application is null)
                    return ServiceResult<object>.NotFound("Offer application not found.");

                if (application.CharityId != charity.CharityId)
                    return ServiceResult<object>
                        .Forbidden("You do not have permission to cancel this application.");

                if (application.Status != ApplicationStatus.Pending)
                    return ServiceResult<object>.Error(
                        "Only pending offer applications can be cancelled.",
                        ErrorCode.INVALID_STATUS,
                        System.Net.HttpStatusCode.UnprocessableEntity);

                await _offerApplicationRepository.DeleteAsync(application);

                return ServiceResult<object>.Success("Offer application cancelled.");
            }
            catch (Exception ex)
            {
                return ServiceResult<object>
                    .Internal("An unexpected error occurred", new { message = ex.Message });
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
                return (null, ServiceResult<T>.NotFound("Need application not found."));

            if (application.CharityNeed.CharityId != charity.CharityId)
                return (null, ServiceResult<T>.Forbidden(
                    "You do not have permission to respond to this application."));

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
                ProductImage = _fileService.BuildFullUrl(need.ProductImage),
                Priority = need.Priority,
                Status = need.Status,
                CreatedAt = need.CreatedAt,
                UpdatedAt = need.UpdatedAt
            };
    }
}