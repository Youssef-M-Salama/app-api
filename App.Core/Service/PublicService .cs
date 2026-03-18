using App.Core.DTO.Request;
using App.Core.DTO.Response;
using App.Core.DTO.ResultPattern;
using App.Core.Domain.RepositoryContracts;
using App.Core.ServiceContracts;

namespace App.Core.Services
{
    /// <summary>
    /// Handles public-facing business logic accessible without authentication.
    /// </summary>
    public class PublicService : IPublicService
    {
        private readonly ICharityNeedRepository _charityNeedRepository;
        private readonly IOfferRepository _offerRepository;
        private readonly ICharityRepository _charityRepository;
        private readonly IDonorOrganizationRepository _donorOrganizationRepository;

        private const int MaxPageSize = 50;

        public PublicService(
            ICharityNeedRepository charityNeedRepository,
            IOfferRepository offerRepository,
            ICharityRepository charityRepository,
            IDonorOrganizationRepository donorOrganizationRepository)
        {
            _charityNeedRepository = charityNeedRepository;
            _offerRepository = offerRepository;
            _charityRepository = charityRepository;
            _donorOrganizationRepository = donorOrganizationRepository;
        }

        /// <inheritdoc/>
        public async Task<ServiceResult<IEnumerable<CharityNeedResponseDto>>> GetApprovedCharityNeedsAsync(
            ApprovedCharityNeedsRequestDto query)
        {
            try
            {
                if (query.Page <= 0)
                    return ServiceResult<IEnumerable<CharityNeedResponseDto>>.InvalidPage();

                if (query.PageSize <= 0)
                    return ServiceResult<IEnumerable<CharityNeedResponseDto>>.InvalidPageSize();

                if (query.PageSize > MaxPageSize)
                    return ServiceResult<IEnumerable<CharityNeedResponseDto>>.PageSizeTooLarge();

                var items = await _charityNeedRepository.GetApprovedCharityNeedsAsync(
                    query.Category,
                    query.City,
                    query.Governorate,
                    query.Search,
                    query.Page,
                    query.PageSize);

                var totalCount = await _charityNeedRepository.CountApprovedCharityNeedsAsync(
                    query.Category,
                    query.City,
                    query.Governorate,
                    query.Search);

                var data = items.Select(cn => new CharityNeedResponseDto
                {
                    CharityNeedId = cn.CharityNeedId,
                    CharityName = cn.Charity.CharityName,
                    ProductName = cn.ProductName,
                    Category = cn.Category,
                    City = cn.Charity.ApplicationUser.City,
                    Governorate = cn.Charity.ApplicationUser.Governorate,
                    Quantity = cn.Quantity,
                    Priority = cn.Priority.ToString(),
                    Status = cn.Status.ToString(),
                    CreatedAt = cn.CreatedAt
                });

                var pagination = PaginationInfo.Create(query.Page, query.PageSize, totalCount);

                return ServiceResult<IEnumerable<CharityNeedResponseDto>>
                    .SuccessPaginated("CharityNeeds retrieved successfully", data, pagination);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<CharityNeedResponseDto>>
                    .Internal("An unexpected error occurred", new { message = ex.Message });
            }
        }

        /// <inheritdoc/>
        public async Task<ServiceResult<StatisticsResponseDto>> GetStatisticsAsync()
        {
            try
            {
                var fulfilledCharityNeeds = await _charityNeedRepository.CountFulfilledCharityNeedsAsync();
                var fulfilledOffers = await _offerRepository.CountFulfilledOffersAsync();
                var activeCharityNeeds = await _charityNeedRepository.CountActiveCharityNeedsAsync();
                var activeOffers = await _offerRepository.CountActiveOffersAsync();
                var totalCharities = await _charityRepository.CountTotalCharitiesAsync();
                var totalDonors = await _donorOrganizationRepository.CountTotalDonorsAsync();
                var itemsFromNeeds = await _charityNeedRepository.SumFulfilledCharityNeedsQuantityAsync();
                var itemsFromOffers = await _offerRepository.SumFulfilledOffersQuantityAsync();

                var data = new StatisticsResponseDto
                {
                    TotalDonations = fulfilledCharityNeeds + fulfilledOffers,
                    TotalCharities = totalCharities,
                    TotalDonors = totalDonors,
                    ActiveCharityNeeds = activeCharityNeeds,
                    ActiveOffers = activeOffers,
                    TotalItemsDonated = itemsFromNeeds + itemsFromOffers
                };

                return ServiceResult<StatisticsResponseDto>
                    .Success("Statistics retrieved successfully", data);
            }
            catch (Exception ex)
            {
                return ServiceResult<StatisticsResponseDto>
                    .Internal("An unexpected error occurred", new { message = ex.Message });
            }
        }
        /// <inheritdoc/>
        public async Task<ServiceResult<IEnumerable<OfferResponseDto>>> GetApprovedOffersAsync(
            ApprovedOffersRequestDto query)
        {
            try
            {
                if (query.Page <= 0)
                    return ServiceResult<IEnumerable<OfferResponseDto>>.InvalidPage();

                if (query.PageSize <= 0)
                    return ServiceResult<IEnumerable<OfferResponseDto>>.InvalidPageSize();

                if (query.PageSize > MaxPageSize)
                    return ServiceResult<IEnumerable<OfferResponseDto>>.PageSizeTooLarge();

                var items = await _offerRepository.GetApprovedOffersAsync(
                    query.Category,
                    query.City,
                    query.Governorate,
                    query.Search,
                    query.Page,
                    query.PageSize);

                var totalCount = await _offerRepository.CountApprovedOffersAsync(
                    query.Category,
                    query.City,
                    query.Governorate,
                    query.Search);

                var data = items.Select(o => new OfferResponseDto
                {
                    OfferId = o.OfferId,
                    DonorOrganizationName = o.DonorOrganization.DonorOrganizationName,
                    ProductName = o.ProductName,
                    Category = o.Category,
                    City = o.DonorOrganization.ApplicationUser.City,
                    Governorate = o.DonorOrganization.ApplicationUser.Governorate,
                    Quantity = o.Quantity,
                    ProductImage = o.ProductImage,
                    ExpiryDate = o.ExpiryDate,
                    Status = o.Status.ToString(),
                    CreatedAt = o.CreatedAt
                });

                var pagination = PaginationInfo.Create(query.Page, query.PageSize, totalCount);

                return ServiceResult<IEnumerable<OfferResponseDto>>
                    .SuccessPaginated("Offers retrieved successfully", data, pagination);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<OfferResponseDto>>
                    .Internal("An unexpected error occurred", new { message = ex.Message });
            }
        }
        /// <inheritdoc/>
        public async Task<ServiceResult<CharityNeedResponseDto>> GetApprovedCharityNeedByIdAsync(Guid charityNeedId)
        {
            try
            {
                var cn = await _charityNeedRepository.GetApprovedCharityNeedByIdAsync(charityNeedId);

                if (cn == null)
                    return ServiceResult<CharityNeedResponseDto>.NotFound(
                        "CharityNeed not found or not approved");

                var data = new CharityNeedResponseDto
                {
                    CharityNeedId = cn.CharityNeedId,
                    CharityName = cn.Charity.CharityName,
                    ProductName = cn.ProductName,
                    Category = cn.Category,
                    City = cn.Charity.ApplicationUser.City,
                    Governorate = cn.Charity.ApplicationUser.Governorate,
                    Quantity = cn.Quantity,
                    Priority = cn.Priority.ToString(),
                    Status = cn.Status.ToString(),
                    CreatedAt = cn.CreatedAt
                };

                return ServiceResult<CharityNeedResponseDto>
                    .Success("CharityNeed retrieved successfully", data);
            }
            catch (Exception ex)
            {
                return ServiceResult<CharityNeedResponseDto>
                    .Internal("An unexpected error occurred", new { message = ex.Message });
            }
        }
        /// <inheritdoc/>
        public async Task<ServiceResult<OfferResponseDto>> GetApprovedOfferByIdAsync(Guid offerId)
        {
            try
            {
                var offer = await _offerRepository.GetApprovedOfferByIdAsync(offerId);

                if (offer == null)
                    return ServiceResult<OfferResponseDto>.NotFound(
                        "Offer not found or not approved");

                var data = new OfferResponseDto
                {
                    OfferId = offer.OfferId,
                    DonorOrganizationName = offer.DonorOrganization.DonorOrganizationName,
                    ProductName = offer.ProductName,
                    Category = offer.Category,
                    City = offer.DonorOrganization.ApplicationUser.City,
                    Governorate = offer.DonorOrganization.ApplicationUser.Governorate,
                    Quantity = offer.Quantity,
                    ProductImage = offer.ProductImage,
                    ExpiryDate = offer.ExpiryDate,
                    Status = offer.Status.ToString(),
                    CreatedAt = offer.CreatedAt
                };

                return ServiceResult<OfferResponseDto>
                    .Success("Offer retrieved successfully", data);
            }
            catch (Exception ex)
            {
                return ServiceResult<OfferResponseDto>
                    .Internal("An unexpected error occurred", new { message = ex.Message });
            }
        }

    }
}   