using App.Core.DTO.Request;
using App.Core.DTO.Response;
using App.Core.DTO.ResultPattern;
using App.Core.RepositoryContracts;
using App.Core.ServiceContracts;

namespace App.Core.Services
{
    /// <summary>
    /// Handles public-facing business logic accessible without authentication.
    /// </summary>
    public class PublicService : IPublicService
    {
        private readonly ICharityNeedRepository _charityNeedRepository;

        private const int MaxPageSize = 50;

        public PublicService(ICharityNeedRepository charityNeedRepository)
        {
            _charityNeedRepository = charityNeedRepository;
        }

        /// <inheritdoc/>
        public async Task<ServiceResult<IEnumerable<CharityNeedResponseDto>>> GetApprovedRequestsAsync(
            GetApprovedRequestsQueryDto query)
        {
            try
            {
                // Validate page
                if (query.Page <= 0)
                    return ServiceResult<IEnumerable<CharityNeedResponseDto>>.InvalidPage();

                // Validate page size
                if (query.PageSize <= 0)
                    return ServiceResult<IEnumerable<CharityNeedResponseDto>>.InvalidPageSize();

                // Validate page size limit
                if (query.PageSize > MaxPageSize)
                    return ServiceResult<IEnumerable<CharityNeedResponseDto>>.PageSizeTooLarge();

                // Fetch data and total count
                var items = await _charityNeedRepository.GetApprovedRequestsAsync(
                    query.Category,
                    query.Search,
                    query.Page,
                    query.PageSize);

                var totalCount = await _charityNeedRepository.CountApprovedRequestsAsync(
                    query.Category,
                    query.Search);

                // Map to response DTOs
                var data = items.Select(cn => new CharityNeedResponseDto
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
                    CreatedAt = cn.CreatedAt
                });

                // Build pagination metadata
                var pagination = PaginationInfo.Create(query.Page, query.PageSize, totalCount);

                return ServiceResult<IEnumerable<CharityNeedResponseDto>>
                    .SuccessPaginated("Requests retrieved successfully", data, pagination);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<CharityNeedResponseDto>>
                    .Internal("An unexpected error occurred", new { message = ex.Message });
            }
        }
    }
}