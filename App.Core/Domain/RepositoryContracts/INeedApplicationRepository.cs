using App.Core.Domain.Entities;

namespace App.Core.Domain.RepositoryContracts
{
    /// <summary>
    /// Repository contract for <see cref="NeedApplication"/> data access.
    /// A NeedApplication is created when a donor organization applies
    /// to fulfill a charity's need.
    /// </summary>
    public interface INeedApplicationRepository
    {
        // =========================================================
        // CHARITY — applications received on my needs
        // =========================================================

        /// <summary>
        /// Returns a paginated list of need applications received by the charity,
        /// i.e. applications where the parent CharityNeed.CharityId == charityId.
        /// Includes DonorOrganization and CharityNeed for mapping.
        /// Ordered by CreatedAt descending.
        /// </summary>
        Task<IEnumerable<NeedApplication>> GetReceivedByCharityIdAsync(
            Guid charityId,
            int page,
            int pageSize);

        /// <summary>
        /// Returns the total count of need applications received by the charity.
        /// </summary>
        Task<int> CountReceivedByCharityIdAsync(Guid charityId);

        /// <summary>
        /// Returns a single need application by its identifier.
        /// Includes CharityNeed (for ownership check) and DonorOrganization.
        /// Returns null if not found.
        /// </summary>
        Task<NeedApplication?> GetByIdAsync(Guid needApplicationId);

        /// <summary>
        /// Persists status changes to an existing need application.
        /// Used when the charity accepts or rejects an application.
        /// </summary>
        Task<NeedApplication> UpdateAsync(NeedApplication application);

        /// <summary>
        /// Creates a new need application.
        /// </summary>
        Task<NeedApplication> CreateAsync(NeedApplication application);

        /// <summary>
        /// Checks if a donor has already applied to a specific charity need.
        /// </summary>
        Task<bool> ExistsAsync(Guid donorOrganizationId, Guid charityNeedId);

        // =========================================================
        // CHARITY DASHBOARD — count breakdown
        // =========================================================

        /// <summary>
        /// Returns a per-status count breakdown of all need applications
        /// received by the given charity.
        /// Used exclusively by the charity dashboard.
        /// </summary>
        Task<(int Total, int Pending, int Accepted, int Rejected)>
            GetReceivedCountsByCharityIdAsync(Guid charityId);

        Task<(int Total, int Pending, int Accepted, int Rejected)> GetSentCountsByDonorOrganizationIdAsync(Guid donorOrganizationId);

        Task<IEnumerable<NeedApplication>> GetSentByDonorOrganizationIdAsync(
            Guid donorOrganizationId,
            int page,
            int pageSize);

        Task<int> CountSentByDonorOrganizationIdAsync(Guid donorOrganizationId);
        
        /// <summary>
        /// Deletes the given need application from the database.
        /// </summary>
        Task DeleteAsync(NeedApplication application);
    }
}