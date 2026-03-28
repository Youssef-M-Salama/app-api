// ICharityNeedRepository.cs
using App.Core.Domain.Entities;

namespace App.Core.Domain.RepositoryContracts
{
    /// <summary>
    /// Repository contract for <see cref="CharityNeed"/> data access.
    /// </summary>
    public interface ICharityNeedRepository
    {
        /// <summary>
        /// Returns a paginated list of approved charity needs, optionally filtered.
        /// </summary>
        /// <param name="category">Optional filter by category (food, clothing, medical, education).</param>
        /// <param name="city">Optional filter by city.</param>
        /// <param name="governorate">Optional filter by governorate.</param>
        /// <param name="search">Optional search term matched against product name.</param>
        /// <param name="page">Page number (1-based).</param>
        /// <param name="pageSize">Number of items per page.</param>
        Task<IEnumerable<CharityNeed>> GetApprovedCharityNeedsAsync(
            string? category,
            string? city,
            string? governorate,
            string? search,
            int page,
            int pageSize);

        /// <summary>
        /// Returns the total count of approved charity needs matching the given filters.
        /// Used by the service layer to build pagination metadata.
        /// </summary>
        /// <param name="category">Optional filter by category.</param>
        /// <param name="city">Optional filter by city.</param>
        /// <param name="governorate">Optional filter by governorate.</param>
        /// <param name="search">Optional search term matched against product name.</param>
        Task<int> CountApprovedCharityNeedsAsync(
            string? category,
            string? city,
            string? governorate,
            string? search);

        /// <summary>
        /// Returns the count of charity needs with status <c>fulfilled</c>.
        /// Used to compute <c>TotalDonations</c> in platform-wide statistics.
        /// </summary>
        Task<int> CountFulfilledCharityNeedsAsync();
        /// <summary>
        /// Returns the count of charity needs with status <c>approved</c>.
        /// Used to compute <c>ActiveCharityNeeds</c> in platform-wide statistics.
        /// </summary>
        Task<int> CountActiveCharityNeedsAsync();
        /// <summary>
        /// Returns the total quantity across all charity needs with status <c>fulfilled</c>.
        /// Used to compute <c>TotalItemsDonated</c> in platform-wide statistics.
        /// </summary>
        Task<int> SumFulfilledCharityNeedsQuantityAsync();
        /// <summary>
        /// Returns a single approved charity need by its identifier.
        /// </summary>
        /// <param name="charityNeedId">The unique identifier of the charity need.</param>
        Task<CharityNeed?> GetApprovedCharityNeedByIdAsync(Guid charityNeedId);
        /// <summary>
        /// Returns a single charity need by its identifier regardless of status.
        /// Includes the parent <see cref="Charity"/> for ownership checks.
        /// Returns null if not found.
        /// </summary>
        Task<CharityNeed?> GetByIdWithCharityAsync(Guid charityNeedId);

        /// <summary>
        /// Returns a paginated list of charity needs belonging to the given charity.
        /// Optionally filtered by status string (case-insensitive).
        /// Ordered by CreatedAt descending.
        /// </summary>
        Task<IEnumerable<CharityNeed>> GetByCharityIdAsync(
            Guid charityId,
            string? status,
            int page,
            int pageSize);

        /// <summary>
        /// Returns the total count of charity needs belonging to the given charity,
        /// optionally filtered by status string.
        /// </summary>
        Task<int> CountByCharityIdAsync(Guid charityId, string? status);

        /// <summary>
        /// Persists a new charity need and returns the saved entity.
        /// </summary>
        Task<CharityNeed> CreateAsync(CharityNeed need);

        /// <summary>
        /// Persists changes to an existing charity need and returns the updated entity.
        /// </summary>
        Task<CharityNeed> UpdateAsync(CharityNeed need);

        /// <summary>
        /// Deletes the given charity need from the database.
        /// </summary>
        Task DeleteAsync(CharityNeed need);

        /// <summary>
        /// Returns a per-status count breakdown for all charity needs
        /// belonging to the given charity.
        /// Used exclusively by the charity dashboard.
        /// </summary>
        Task<(int Total, int Pending, int Approved, int Rejected, int Fulfilled)>
            GetNeedCountsByCharityIdAsync(Guid charityId);
    }
}