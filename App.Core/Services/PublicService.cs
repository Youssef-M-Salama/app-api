using App.Core.DTOs.Request;
using App.Core.DTOs.Response;
using App.Core.DTOs.ResultPattern;
using App.Core.Domain.RepositoryContracts;
using App.Core.ServiceContracts;
using Microsoft.Extensions.Logging;

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
        private readonly IFileService _fileService;
        private readonly ILogger<PublicService> _logger;

        private const int MaxPageSize = 50;

        public PublicService(
            ICharityNeedRepository charityNeedRepository,
            IOfferRepository offerRepository,
            ICharityRepository charityRepository,
            IDonorOrganizationRepository donorOrganizationRepository,
            IFileService fileService,
            ILogger<PublicService> logger)
        {
            _charityNeedRepository = charityNeedRepository;
            _offerRepository = offerRepository;
            _charityRepository = charityRepository;
            _donorOrganizationRepository = donorOrganizationRepository;
            _fileService = fileService;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<ServiceResult<IEnumerable<CharityNeedResponseDTO>>> GetApprovedCharityNeedsAsync(
            ApprovedCharityNeedsRequestDTO query)
        {
            try
            {
                if (query.Page <= 0)
                    return ServiceResult<IEnumerable<CharityNeedResponseDTO>>.InvalidPage();

                if (query.PageSize <= 0)
                    return ServiceResult<IEnumerable<CharityNeedResponseDTO>>.InvalidPageSize();

                if (query.PageSize > MaxPageSize)
                    return ServiceResult<IEnumerable<CharityNeedResponseDTO>>.PageSizeTooLarge();

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

                var data = items.Select(cn => new CharityNeedResponseDTO
                {
                    CharityNeedId = cn.CharityNeedId,
                    CharityName = cn.Charity.CharityName,
                    ProductName = cn.ProductName,
                    Category = cn.Category,
                    City = cn.Charity.ApplicationUser.City,
                    Governorate = cn.Charity.ApplicationUser.Governorate,
                    Quantity = cn.Quantity,
                    Priority = cn.Priority,
                    Status = cn.Status,
                    CreatedAt = cn.CreatedAt,
                    ProductImage = _fileService.GetImageUrl(cn.ProductImage)
                });

                var pagination = PaginationInfo.Create(query.Page, query.PageSize, totalCount);

                return ServiceResult<IEnumerable<CharityNeedResponseDTO>>
                    .SuccessPaginated("تم استرجاع احتياجات الجمعيات بنجاح", data, pagination);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in PublicService");
                return ServiceResult<IEnumerable<CharityNeedResponseDTO>>
                    .Internal("حدث خطأ غير متوقع", new { message = ex.Message });
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
                    .Success("تم استرجاع الإحصائيات بنجاح", data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in PublicService");
                return ServiceResult<StatisticsResponseDto>
                    .Internal("حدث خطأ غير متوقع", new { message = ex.Message });
            }
        }
        /// <inheritdoc/>
        public async Task<ServiceResult<IEnumerable<OfferResponseDTO>>> GetApprovedOffersAsync(
            ApprovedOffersRequestDTO query)
        {
            try
            {
                if (query.Page <= 0)
                    return ServiceResult<IEnumerable<OfferResponseDTO>>.InvalidPage();

                if (query.PageSize <= 0)
                    return ServiceResult<IEnumerable<OfferResponseDTO>>.InvalidPageSize();

                if (query.PageSize > MaxPageSize)
                    return ServiceResult<IEnumerable<OfferResponseDTO>>.PageSizeTooLarge();

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

                var data = items.Select(o => new OfferResponseDTO
                {
                    OfferId = o.OfferId,
                    DonorOrganizationName = o.DonorOrganization.DonorOrganizationName,
                    ProductName = o.ProductName,
                    Category = o.Category,
                    City = o.DonorOrganization.ApplicationUser.City,
                    Governorate = o.DonorOrganization.ApplicationUser.Governorate,
                    Quantity = o.Quantity,
                    ProductImage = _fileService.GetImageUrl(o.ProductImage),
                    ExpiryDate = o.ExpiryDate,
                    Status = o.Status,
                    CreatedAt = o.CreatedAt
                });

                var pagination = PaginationInfo.Create(query.Page, query.PageSize, totalCount);

                return ServiceResult<IEnumerable<OfferResponseDTO>>
                    .SuccessPaginated("تم استرجاع العروض بنجاح", data, pagination);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in PublicService");
                return ServiceResult<IEnumerable<OfferResponseDTO>>
                    .Internal("حدث خطأ غير متوقع", new { message = ex.Message });
            }
        }
        /// <inheritdoc/>
        public async Task<ServiceResult<CharityNeedResponseDTO>> GetApprovedCharityNeedByIdAsync(Guid charityNeedId)
        {
            try
            {
                var cn = await _charityNeedRepository.GetApprovedCharityNeedByIdAsync(charityNeedId);

                if (cn == null)
                    return ServiceResult<CharityNeedResponseDTO>.NotFound(
                        "احتياج الجمعية غير موجود أو لم تتم الموافقة عليه بعد");

                var data = new CharityNeedResponseDTO
                {
                    CharityNeedId = cn.CharityNeedId,
                    CharityName = cn.Charity.CharityName,
                    ProductName = cn.ProductName,
                    Category = cn.Category,
                    City = cn.Charity.ApplicationUser.City,
                    Governorate = cn.Charity.ApplicationUser.Governorate,
                    Quantity = cn.Quantity,
                    Priority = cn.Priority,
                    Status = cn.Status,
                    CreatedAt = cn.CreatedAt,
                    ProductImage = _fileService.GetImageUrl(cn.ProductImage)
                };

                return ServiceResult<CharityNeedResponseDTO>
                    .Success("تم استرجاع احتياج الجمعية بنجاح", data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in PublicService");
                return ServiceResult<CharityNeedResponseDTO>
                    .Internal("حدث خطأ غير متوقع", new { message = ex.Message });
            }
        }
        /// <inheritdoc/>
        public async Task<ServiceResult<OfferResponseDTO>> GetApprovedOfferByIdAsync(Guid offerId)
        {
            try
            {
                var offer = await _offerRepository.GetApprovedOfferByIdAsync(offerId);

                if (offer == null)
                    return ServiceResult<OfferResponseDTO>.NotFound(
                        "العرض غير موجود أو لم تتم الموافقة عليه بعد");

                var data = new OfferResponseDTO
                {
                    OfferId = offer.OfferId,
                    DonorOrganizationName = offer.DonorOrganization.DonorOrganizationName,
                    ProductName = offer.ProductName,
                    Category = offer.Category,
                    City = offer.DonorOrganization.ApplicationUser.City,
                    Governorate = offer.DonorOrganization.ApplicationUser.Governorate,
                    Quantity = offer.Quantity,
                    ProductImage = _fileService.GetImageUrl(offer.ProductImage),
                    ExpiryDate = offer.ExpiryDate,
                    Status = offer.Status,
                    CreatedAt = offer.CreatedAt
                };

                return ServiceResult<OfferResponseDTO>
                    .Success("تم استرجاع العرض بنجاح", data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in PublicService");
                return ServiceResult<OfferResponseDTO>
                    .Internal("حدث خطأ غير متوقع", new { message = ex.Message });
            }
        }

    }
}   