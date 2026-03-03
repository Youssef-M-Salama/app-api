using App.Core.Domain.Entities;

namespace App.Core.RepositoryContracts
{
    /// <summary>
    /// Repository contract for charity need (request) data access.
    /// </summary>
    public interface ICharityNeedRepository
    {
        /// <summary>
        /// Returns a paginated list of approved charity needs.
        /// </summary>
        /// <param name="category">Optional filter by category (food, clothing, medical, education).</param>
        /// <param name="search">Optional search term matched against product name.</param>
        /// <param name="page">Page number (1-based).</param>
        /// <param name="pageSize">Number of items per page.</param>
        Task<IEnumerable<CharityNeed>> GetApprovedRequestsAsync(
            string? category,
            string? search,
            int page,
            int pageSize);

        /// <summary>
        /// Returns the total count of approved charity needs matching the given filters.
        /// Used by the service layer to build pagination metadata.
        /// </summary>
        /// <param name="category">Optional filter by category (food, clothing, medical, education).</param>
        /// <param name="search">Optional search term matched against product name.</param>
        Task<int> CountApprovedRequestsAsync(
            string? category,
            string? search);
    }
}