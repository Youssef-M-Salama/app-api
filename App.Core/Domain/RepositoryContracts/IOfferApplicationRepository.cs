using App.Core.Domain.Entities;

namespace App.Core.Domain.RepositoryContracts
{
    /// <summary>
    /// Repository contract for <see cref="OfferApplication"/> data access.
    /// An OfferApplication is created when a charity applies to a donor's offer.
    /// </summary>
    public interface IOfferApplicationRepository
    {
        // =========================================================
        // CHARITY — applications sent to offers
        // =========================================================

        /// <summary>
        /// Returns a paginated list of offer applications sent by the charity.
        /// Includes Offer and DonorOrganization for mapping.
        /// Ordered by CreatedAt descending.
        /// </summary>
        Task<IEnumerable<OfferApplication>> GetSentByCharityIdAsync(
            Guid charityId,
            int page,
            int pageSize);

        /// <summary>
        /// Returns the total count of offer applications sent by the charity.
        /// </summary>
        Task<int> CountSentByCharityIdAsync(Guid charityId);

        /// <summary>
        /// Returns a single offer application by its identifier.
        /// Includes Offer and DonorOrganization for mapping and ownership check.
        /// Returns null if not found.
        /// </summary>
        Task<OfferApplication?> GetByIdAsync(Guid offerApplicationId);

        /// <summary>
        /// Returns true if the charity has already applied to the given offer.
        /// Used to enforce the unique application constraint at the service layer
        /// before hitting the database unique index.
        /// </summary>
        Task<bool> ExistsAsync(Guid charityId, Guid offerId);

        /// <summary>
        /// Persists a new offer application and returns the saved entity.
        /// </summary>
        Task<OfferApplication> CreateAsync(OfferApplication application);

        /// <summary>
        /// Deletes the given offer application from the database.
        /// </summary>
        Task DeleteAsync(OfferApplication application);

        // =========================================================
        // CHARITY DASHBOARD — count breakdown
        // =========================================================

        /// <summary>
        /// Returns a per-status count breakdown of all offer applications
        /// sent by the given charity.
        /// Used exclusively by the charity dashboard.
        /// </summary>
        Task<(int Total, int Pending, int Accepted, int Rejected)>
            GetSentCountsByCharityIdAsync(Guid charityId);
    }
}