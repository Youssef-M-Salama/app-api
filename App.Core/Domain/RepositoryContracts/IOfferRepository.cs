using App.Core.Domain.Entities;

namespace App.Core.Domain.RepositoryContracts
{
    /// <summary>
    /// Repository contract for <see cref="Offer"/> data access.
    /// </summary>
    public interface IOfferRepository
    {
        /// <summary>
        /// Returns a paginated list of approved offers, optionally filtered.
        /// </summary>
        /// <param name="category">Optional filter by category (food, clothing, medical, education).</param>
        /// <param name="city">Optional filter by city.</param>
        /// <param name="governorate">Optional filter by governorate.</param>
        /// <param name="search">Optional search term matched against product name.</param>
        /// <param name="page">Page number (1-based).</param>
        /// <param name="pageSize">Number of items per page.</param>
        Task<IEnumerable<Offer>> GetApprovedOffersAsync(
            string? category,
            string? city,
            string? governorate,
            string? search,
            int page,
            int pageSize);

        /// <summary>
        /// Returns the total count of approved offers matching the given filters.
        /// Used by the service layer to build pagination metadata.
        /// </summary>
        /// <param name="category">Optional filter by category.</param>
        /// <param name="city">Optional filter by city.</param>
        /// <param name="governorate">Optional filter by governorate.</param>
        /// <param name="search">Optional search term matched against product name.</param>
        Task<int> CountApprovedOffersAsync(
            string? category,
            string? city,
            string? governorate,
            string? search);

        /// <summary>
        /// Returns the count of offers with status <c>Fulfilled</c>.
        /// Used to compute <c>TotalDonations</c> in platform-wide statistics.
        /// </summary>
        Task<int> CountFulfilledOffersAsync();

        /// <summary>
        /// Returns the count of offers with status <c>Approved</c>.
        /// Used to compute <c>ActiveOffers</c> in platform-wide statistics.
        /// </summary>
        Task<int> CountActiveOffersAsync();

        /// <summary>
        /// Returns the total quantity across all offers with status <c>Fulfilled</c>.
        /// Used to compute <c>TotalItemsDonated</c> in platform-wide statistics.
        /// </summary>
        Task<int> SumFulfilledOffersQuantityAsync();
        /// <summary>
        /// Returns a single approved offer by its identifier.
        /// </summary>
        /// <param name="offerId">The unique identifier of the offer.</param>
        Task<Offer?> GetApprovedOfferByIdAsync(Guid offerId);
    }
}