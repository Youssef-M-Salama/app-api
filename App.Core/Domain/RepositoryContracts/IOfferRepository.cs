using App.Core.Domain.Entities;
using App.Core.Enums;

namespace App.Core.Domain.RepositoryContracts
{
    /// <summary>
    /// Repository contract for <see cref="Offer"/> data access.
    /// </summary>
    public interface IOfferRepository
    {
        Task<(int Total, int Pending, int Approved, int Rejected, int Fulfilled, int Expired)> GetOfferCountsByDonorOrganizationIdAsync(Guid charityId);

        /// <summary>
        /// Returns a paginated list of approved offers, optionally filtered.
        /// </summary>
        Task<IEnumerable<Offer>> GetApprovedOffersAsync(
        ProductCategory? category,
        string? city,
        string? governorate,
        string? search,
        int page,
        int pageSize);

        /// <summary>
        /// Returns the total count of approved offers matching the given filters.
        /// Used by the service layer to build pagination metadata.
        /// </summary>
        Task<int> CountApprovedOffersAsync(
            ProductCategory? category,
            string? city,
            string? governorate,
            string? search);

        /// <summary>
        /// Returns the count of offers with status <c>Fulfilled</c>.
        /// Used to compute <c>TotalDoneDonation</c> in platform-wide statistics.
        /// </summary>
        Task<int> CountFulfilledOffersAsync();

        /// <summary>
        /// Returns the count of offers with status <c>Approved</c>.
        /// Used to compute <c>ActiveOffers</c> in platform-wide statistics.
        /// </summary>
        Task<int> CountActiveOffersAsync();

        /// <summary>
        /// Returns the total quantity across all offers with status <c>Fulfilled</c>.
        /// Used to compute <c>TotalDonations</c> in platform-wide statistics.
        /// </summary>
        Task<int> SumFulfilledOffersQuantityAsync();
        /// <summary>
        /// Returns a single approved offer by its identifier.
        /// </summary>
        /// <param name="offerId">The unique identifier of the offer.</param>
        Task<Offer?> GetApprovedOfferByIdAsync(Guid offerId);
        Task<IEnumerable<Offer>> GetByDonorOrganizationIdAsync(
            Guid donorId,
            OfferStatus? status,
            int page,
            int pageSize);

        Task<int> CountByDonorOrganizationIdAsync(Guid donorId, OfferStatus? status);

        Task<Offer?> GetByIdWithDonorAsync(Guid offerId);

        Task UpdateAsync(Offer offer);

        Task DeleteAsync(Offer offer);

        /// <summary>
        /// Persists a new offer and returns the saved entity.
        /// </summary>
        Task<Offer> CreateAsync(Offer offer);
    }
}