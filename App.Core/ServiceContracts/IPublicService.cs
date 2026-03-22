using App.Core.DTOs.Request;
using App.Core.DTOs.Response;
using App.Core.DTOs.ResultPattern;

namespace App.Core.ServiceContracts
{
    /// <summary>
    /// Service contract for public-facing endpoints accessible without authentication.
    /// </summary>
    public interface IPublicService
    {
        /// <summary>
        /// Returns a paginated list of approved charity needs visible to the public.
        /// </summary>
        /// <param name="query">Filtering and pagination parameters.</param>
        /// <returns>
        /// ServiceResult containing:
        /// - Success (200 OK): Paginated list of approved charity needs.
        /// - Bad Request (400): Invalid page or page size.
        /// - Internal Error (500): Unexpected server error.
        /// </returns>
        Task<ServiceResult<IEnumerable<CharityNeedResponseDTO>>> GetApprovedCharityNeedsAsync(
            ApprovedCharityNeedsRequestDTO query);

        Task<ServiceResult<StatisticsResponseDto>> GetStatisticsAsync();
        /// <summary>
        /// Returns a paginated list of approved offers visible to the public.
        /// </summary>
        /// <param name="query">Filtering and pagination parameters.</param>
        /// <returns>
        /// ServiceResult containing:
        /// - Success (200 OK): Paginated list of approved offers.
        /// - Bad Request (400): Invalid page or page size.
        /// - Internal Error (500): Unexpected server error.
        /// </returns>
        Task<ServiceResult<IEnumerable<OfferResponseDto>>> GetApprovedOffersAsync(
            ApprovedOffersRequestDTO query);
        /// <summary>
        /// Returns a single approved charity need by its identifier.
        /// </summary>
        /// <param name="charityNeedId">The unique identifier of the charity need.</param>
        /// <returns>
        /// ServiceResult containing:
        /// - Success (200 OK): Charity need details.
        /// - Not Found (404): Charity need not found or not approved.
        /// - Internal Error (500): Unexpected server error.
        /// </returns>
        Task<ServiceResult<CharityNeedResponseDTO>> GetApprovedCharityNeedByIdAsync(Guid charityNeedId);
        /// <summary>
        /// Returns a single approved offer by its identifier.
        /// </summary>
        /// <param name="offerId">The unique identifier of the offer.</param>
        /// <returns>
        /// ServiceResult containing:
        /// - Success (200 OK): Offer details.
        /// - Not Found (404): Offer not found or not approved.
        /// - Internal Error (500): Unexpected server error.
        /// </returns>
        Task<ServiceResult<OfferResponseDto>> GetApprovedOfferByIdAsync(Guid offerId);
    }
}