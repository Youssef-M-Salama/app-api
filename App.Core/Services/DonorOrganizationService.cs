using App.Core.Domain.Entities;
using App.Core.Domain.RepositoryContracts;
using App.Core.DTOs.Request;
using App.Core.DTOs.Response;
using App.Core.DTOs.ResultPattern;
using App.Core.Enums;
using App.Core.ServiceContracts;
using Microsoft.AspNetCore.Http.HttpResults;

namespace App.Core.Services
{
    public class DonorOrganizationService : IDonorOrganizationService
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IOfferRepository _offerRepository;
        private readonly IOfferApplicationRepository _offerApplicationRepository;
        private readonly INeedApplicationRepository _needApplicationRepository;
        private readonly IFileService _fileService;

        public DonorOrganizationService(IProfileRepository profileRepository, IOfferRepository offerRepository, IOfferApplicationRepository offerApplicationRepository, INeedApplicationRepository needApplicationRepository, IFileService fileService)
        {
            _profileRepository = profileRepository;
            _offerRepository = offerRepository;
            _offerApplicationRepository = offerApplicationRepository;
            _needApplicationRepository = needApplicationRepository;
            _fileService = fileService;
        }

        public async Task<ServiceResult<DonorDashboardResponseDTO>> GetDashboardAsync(Guid userId)
        {
            try
            {
                var Donor = await _profileRepository.GetDonorOrganizationByUserIdAsync(userId);
                if (Donor is null)
                {
                    return ServiceResult<DonorDashboardResponseDTO>.NotFound("Donor Organization profile not found.");
                }
                var offerCountsTask = _offerRepository
                    .GetOfferCountsByDonorOrganizationIdAsync(Donor.DonorOrganizationId);

                var receivedCountsTask = _offerApplicationRepository
                    .GetReceivedCountsByDonorOrganizationIdAsync(Donor.DonorOrganizationId);

                var sentCountsTask = _needApplicationRepository
                    .GetSentCountsByDonorOrganizationIdAsync(Donor.DonorOrganizationId);
                await Task.WhenAll(offerCountsTask, receivedCountsTask, sentCountsTask);

                var offerCounts = offerCountsTask.Result;
                var receivedCounts = receivedCountsTask.Result;
                var sentCounts = sentCountsTask.Result;
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
                   .Success("Dashboard statistics retrieved successfully", data);

            }
            catch (Exception ex)
            {
                return ServiceResult<DonorDashboardResponseDTO>
                  .Internal("An unexpected error occurred", new { message = ex.Message });
            }
        }
        public async Task<ServiceResult<OfferDetailResponseDTO>> CreateOfferAsync(Guid userId, CreateOfferRequestDTO request)
        {
            try
            {
                var Donor = await _profileRepository.GetDonorOrganizationByUserIdAsync(userId);
                if (Donor is null)
                {
                    return ServiceResult<OfferDetailResponseDTO>.NotFound("Donor Organization profile not found.");
                }
                if (!Donor.IsVerified || !Donor.IsActive)
                {
                    return ServiceResult<OfferDetailResponseDTO>.Forbidden("Your donor account must be verified and active to post offers.");
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
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                var created=await _offerRepository.CreateAsync(offer);
                return ServiceResult<OfferDetailResponseDTO>
                    .Created("Offer created successfully. It is now pending admin approval.",
                             MapToOfferDetailDTO(created));
            }
            catch (Exception ex)
            {
                return ServiceResult<OfferDetailResponseDTO>
                  .Internal("An unexpected error occurred", new { message = ex.Message });
            }
        }
        
        public async Task<ServiceResult<IEnumerable<OfferDetailResponseDTO>>> GetMyOffersAsync(Guid userId, MyOffersFilterDTO query)
        {
            try
            {
                if (query.Page <= 0) return ServiceResult<IEnumerable<OfferDetailResponseDTO>>.InvalidPage();
                if (query.PageSize <= 0) return ServiceResult<IEnumerable<OfferDetailResponseDTO>>.InvalidPageSize();

                var donor = await _profileRepository.GetDonorOrganizationByUserIdAsync(userId);
                if (donor is null) return ServiceResult<IEnumerable<OfferDetailResponseDTO>>.NotFound("Donor not found.");

                var items = await _offerRepository.GetByDonorOrganizationIdAsync(
                    donor.DonorOrganizationId, query.Status, query.Page, query.PageSize);
                var totalCount = await _offerRepository.CountByDonorOrganizationIdAsync(
                    donor.DonorOrganizationId, query.Status);

                var data = items.Select(MapToOfferDetailDTO);
                var pagination = PaginationInfo.Create(query.Page, query.PageSize, totalCount);

                return ServiceResult<IEnumerable<OfferDetailResponseDTO>>
                    .SuccessPaginated("Offers retrieved successfully", data, pagination);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<OfferDetailResponseDTO>>.Internal("Error", new { message = ex.Message });
            }
        }

        public async Task<ServiceResult<OfferDetailResponseDTO>> GetMyOfferByIdAsync(Guid userId, Guid offerId)
        {
            try
            {
                var donor = await _profileRepository.GetDonorOrganizationByUserIdAsync(userId);
                if (donor is null) return ServiceResult<OfferDetailResponseDTO>.NotFound("Donor not found.");

                var offer = await _offerRepository.GetByIdWithDonorAsync(offerId);
                if (offer is null || offer.DonorOrganizationId != donor.DonorOrganizationId)
                    return ServiceResult<OfferDetailResponseDTO>.NotFound("Offer not found.");

                return ServiceResult<OfferDetailResponseDTO>.Success("Offer retrieved", MapToOfferDetailDTO(offer));
            }
            catch (Exception ex)
            {
                return ServiceResult<OfferDetailResponseDTO>.Internal("Error", new { message = ex.Message });
            }
        }

        public async Task<ServiceResult<object>> UpdateOfferAsync(Guid userId, Guid offerId, UpdateOfferRequestDTO request)
        {
            try
            {
                var donor = await _profileRepository.GetDonorOrganizationByUserIdAsync(userId);
                if (donor is null) return ServiceResult<object>.NotFound("Donor not found.");

                var offer = await _offerRepository.GetByIdWithDonorAsync(offerId);
                if (offer is null || offer.DonorOrganizationId != donor.DonorOrganizationId)
                    return ServiceResult<object>.NotFound("Offer not found.");

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

                offer.UpdatedAt = DateTime.UtcNow;
                await _offerRepository.UpdateAsync(offer);

                return ServiceResult<object>.Success("Offer updated successfully.");
            }
            catch (Exception ex)
            {
                return ServiceResult<object>.Internal("Error", new { message = ex.Message });
            }
        }

        public async Task<ServiceResult<object>> DeleteOfferAsync(Guid userId, Guid offerId)
        {
            try
            {
                var donor = await _profileRepository.GetDonorOrganizationByUserIdAsync(userId);
                if (donor is null) return ServiceResult<object>.NotFound("Donor not found.");

                var offer = await _offerRepository.GetByIdWithDonorAsync(offerId);
                if (offer is null || offer.DonorOrganizationId != donor.DonorOrganizationId)
                    return ServiceResult<object>.NotFound("Offer not found.");

                if (offer.Status != OfferStatus.Pending)
                    return ServiceResult<object>.Error("Only pending offers can be deleted.", ErrorCode.INVALID_STATUS, System.Net.HttpStatusCode.UnprocessableEntity);

                await _fileService.DeleteImageAsync(offer.ProductImage);
                await _offerRepository.DeleteAsync(offer);

                return ServiceResult<object>.Success("Offer deleted successfully.");
            }
            catch (Exception ex)
            {
                return ServiceResult<object>.Internal("Error", new { message = ex.Message });
            }
        }

        public async Task<ServiceResult<object>> FulfillOfferAsync(Guid userId, Guid offerId)
        {
            try
            {
                var donor = await _profileRepository.GetDonorOrganizationByUserIdAsync(userId);
                if (donor is null) return ServiceResult<object>.NotFound("Donor not found.");

                var offer = await _offerRepository.GetByIdWithDonorAsync(offerId);
                if (offer is null || offer.DonorOrganizationId != donor.DonorOrganizationId)
                    return ServiceResult<object>.NotFound("Offer not found.");

                if (offer.Status != OfferStatus.Approved)
                    return ServiceResult<object>.Error("Only approved offers can be fulfilled.", ErrorCode.INVALID_STATUS, System.Net.HttpStatusCode.UnprocessableEntity);

                offer.Status = OfferStatus.Fulfilled;
                offer.UpdatedAt = DateTime.UtcNow;
                await _offerRepository.UpdateAsync(offer);

                return ServiceResult<object>.Success("Offer fulfilled.");
            }
            catch (Exception ex)
            {
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
                if (donor is null) return ServiceResult<IEnumerable<MyOfferApplicationResponseDTO>>.NotFound("Donor not found.");

                var items = await _offerApplicationRepository.GetReceivedByDonorOrganizationIdAsync(
                    donor.DonorOrganizationId, query.Page, query.PageSize);
                    
                var counts = await _offerApplicationRepository.GetReceivedCountsByDonorOrganizationIdAsync(donor.DonorOrganizationId);

                var data = items.Select(oa => new MyOfferApplicationResponseDTO
                {
                    OfferApplicationId = oa.OfferApplicationId,
                    OfferId = oa.OfferId,
                    ProductName = oa.Offer.ProductName,
                    DonorOrganizationName = oa.Charity.CharityName, // For charity applications, showing Charity Name instead
                    Status = oa.Status,
                    CreatedAt = oa.CreatedAt
                });

                var pagination = PaginationInfo.Create(query.Page, query.PageSize, counts.Total);

                return ServiceResult<IEnumerable<MyOfferApplicationResponseDTO>>
                    .SuccessPaginated("Applications retrieved successfully", data, pagination);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<MyOfferApplicationResponseDTO>>.Internal("Error", new { message = ex.Message });
            }
        }

        public async Task<ServiceResult<object>> AcceptOfferApplicationAsync(Guid userId, Guid offerApplicationId)
        {
            try
            {
                var donor = await _profileRepository.GetDonorOrganizationByUserIdAsync(userId);
                if (donor is null) return ServiceResult<object>.NotFound("Donor not found.");

                var application = await _offerApplicationRepository.GetByIdAsync(offerApplicationId);
                if (application is null || application.Offer.DonorOrganizationId != donor.DonorOrganizationId)
                    return ServiceResult<object>.NotFound("Application not found.");

                if (application.Status != ApplicationStatus.Pending)
                    return ServiceResult<object>.Error("Only pending applications can be accepted.", ErrorCode.INVALID_STATUS, System.Net.HttpStatusCode.UnprocessableEntity);

                application.Status = ApplicationStatus.Accepted;
                application.UpdatedAt = DateTime.UtcNow;
                await _offerApplicationRepository.UpdateAsync(application);

                return ServiceResult<object>.Success("Application accepted.");
            }
            catch (Exception ex)
            {
                return ServiceResult<object>.Internal("Error", new { message = ex.Message });
            }
        }

        public async Task<ServiceResult<object>> RejectOfferApplicationAsync(Guid userId, Guid offerApplicationId)
        {
            try
            {
                var donor = await _profileRepository.GetDonorOrganizationByUserIdAsync(userId);
                if (donor is null) return ServiceResult<object>.NotFound("Donor not found.");

                var application = await _offerApplicationRepository.GetByIdAsync(offerApplicationId);
                if (application is null || application.Offer.DonorOrganizationId != donor.DonorOrganizationId)
                    return ServiceResult<object>.NotFound("Application not found.");

                if (application.Status != ApplicationStatus.Pending)
                    return ServiceResult<object>.Error("Only pending applications can be rejected.", ErrorCode.INVALID_STATUS, System.Net.HttpStatusCode.UnprocessableEntity);

                application.Status = ApplicationStatus.Rejected;
                application.UpdatedAt = DateTime.UtcNow;
                await _offerApplicationRepository.UpdateAsync(application);

                return ServiceResult<object>.Success("Application rejected.");
            }
            catch (Exception ex)
            {
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
                ProductImage = offer.ProductImage,
                ExpiryDate = offer.ExpiryDate,
                Status = offer.Status,
                CreatedAt = offer.CreatedAt,
                UpdatedAt = offer.UpdatedAt
            };
        }
    }
}
