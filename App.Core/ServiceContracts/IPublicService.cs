using App.Core.DTO.Request;
using App.Core.DTO.Response;
using App.Core.DTO.ResultPattern;

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
        Task<ServiceResult<IEnumerable<CharityNeedResponseDto>>> GetApprovedRequestsAsync(
            GetApprovedRequestsQueryDto query);
    }
}