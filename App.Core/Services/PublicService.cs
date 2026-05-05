using App.Core.Domain.RepositoryContracts;
using App.Core.DTOs.Request;
using App.Core.DTOs.Response;
using App.Core.DTOs.ResultPattern;
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
        private readonly ICacheService _cacheService;

        private const int MaxPageSize = 50;

        public PublicService(
            ICharityNeedRepository charityNeedRepository,
            IOfferRepository offerRepository,
            ICharityRepository charityRepository,
            IDonorOrganizationRepository donorOrganizationRepository,
            IFileService fileService,
            ILogger<PublicService> logger,
            ICacheService cacheService)
        {
            _charityNeedRepository = charityNeedRepository;
            _offerRepository = offerRepository;
            _charityRepository = charityRepository;
            _donorOrganizationRepository = donorOrganizationRepository;
            _fileService = fileService;
            _logger = logger;
            _cacheService = cacheService;
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

                var cacheKey = $"charityneeds:approved:{query.Category}:{query.City}:{query.Governorate}:{query.Search}:p{query.Page}:s{query.PageSize}";
                var cachedData = await _cacheService.GetAsync<CachedCharityNeedsDataDTO>(cacheKey);

                if (cachedData?.CharityNeeds?.Any() == true)
                {
                    return ServiceResult<IEnumerable<CharityNeedResponseDTO>>
                        .SuccessPaginated("تم استرجاع احتياجات الجمعيات بنجاح", cachedData.CharityNeeds, cachedData.Pagination);
                }

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
                    Unit = cn.Unit,
                    Priority = cn.Priority,
                    Status = cn.Status,
                    Email = cn.Charity.ApplicationUser.Email,
                    Phone = cn.Charity.ApplicationUser.PhoneNumber,
                    Whatsapp = cn.Charity.ApplicationUser.Whatsapp,
                    Description = cn.Description,
                    CharityDescription = cn.Charity.CharityDescription,
                    CreatedAt = cn.CreatedAt,
                    ProductImage = _fileService.GetImageUrl(cn.ProductImage)
                }).ToList(); // Materialize the query

                var pagination = PaginationInfo.Create(query.Page, query.PageSize, totalCount);

                var cacheDataToStore = new CachedCharityNeedsDataDTO
                {
                    CharityNeeds = data,
                    Pagination = pagination
                };

                await _cacheService.SetAsync(cacheKey, cacheDataToStore, TimeSpan.FromMinutes(5));

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
                var data = new StatisticsResponseDto
                {
                    TotalCharities = totalCharities,
                    TotalDonors = totalDonors,
                    ActiveCharityNeeds = activeCharityNeeds,
                    ActiveOffers = activeOffers,
                    TotalDoneDonation = fulfilledCharityNeeds + fulfilledOffers
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



                var cacheKey = $"offers:approved:{query.Category}:{query.City}:{query.Governorate}:{query.Search}:p{query.Page}:s{query.PageSize}";

                var cachedData = await _cacheService.GetAsync<CachedOffersDataDTO>(cacheKey);

                if (cachedData?.Offers?.Any() == true)
                {
                    return ServiceResult<IEnumerable<OfferResponseDTO>>
                        .SuccessPaginated("تم استرجاع العروض بنجاح", cachedData.Offers, cachedData.Pagination);
                }

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
                    Unit = o.Unit,
                    ProductImage = _fileService.GetImageUrl(o.ProductImage),
                    ExpiryDate = o.ExpiryDate,
                    Status = o.Status,
                    Email = o.DonorOrganization.ApplicationUser.Email,
                    Phone = o.DonorOrganization.ApplicationUser.PhoneNumber,
                    Whatsapp = o.DonorOrganization.ApplicationUser.Whatsapp,
                    Description = o.Description,
                    DonorOraganizationDesctption = o.DonorOrganization.DonorOrganizationDescription,
                    CreatedAt = o.CreatedAt
                }).ToList(); // Materialize the query

                var pagination = PaginationInfo.Create(query.Page, query.PageSize, totalCount);

                var cacheDataToStore = new CachedOffersDataDTO
                {
                    Offers = data,
                    Pagination = pagination
                };

                await _cacheService.SetAsync(cacheKey, cacheDataToStore, TimeSpan.FromMinutes(5));

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
                    Unit = cn.Unit,
                    Priority = cn.Priority,
                    Status = cn.Status,
                    Email = cn.Charity.ApplicationUser.Email,
                    Phone = cn.Charity.ApplicationUser.PhoneNumber,
                    Whatsapp = cn.Charity.ApplicationUser.Whatsapp,
                    Description = cn.Description,
                    CharityDescription = cn.Charity.CharityDescription,
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
                    Unit = offer.Unit,
                    ProductImage = _fileService.GetImageUrl(offer.ProductImage),
                    ExpiryDate = offer.ExpiryDate,
                    Status = offer.Status,
                    Email = offer.DonorOrganization.ApplicationUser.Email,
                    Phone = offer.DonorOrganization.ApplicationUser.PhoneNumber,
                    Whatsapp = offer.DonorOrganization.ApplicationUser.Whatsapp,
                    Description = offer.Description,
                    DonorOraganizationDesctption = offer.DonorOrganization.DonorOrganizationDescription,
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