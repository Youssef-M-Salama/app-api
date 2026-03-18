using App.Core.Domain.Entities;
using App.Core.Domain.RepositoryContracts;
using App.Core.Enums;
using App.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace App.Infrastructure.Repository
{
    public class OfferRepository : IOfferRepository
    {
        private readonly ApplicationDbContext _context;

        public OfferRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Offer>> GetApprovedOffersAsync(
            string? category,
            string? city,
            string? governorate,
            string? search,
            int page,
            int pageSize)
        {
            return await BuildApprovedQuery(category, city, governorate, search)
                .OrderByDescending(o => o.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<int> CountApprovedOffersAsync(
            string? category,
            string? city,
            string? governorate,
            string? search)
        {
            return await BuildApprovedQuery(category, city, governorate, search).CountAsync();
        }

        /// <inheritdoc/>
        public async Task<int> CountActiveOffersAsync()
        {
            return await _context.Offers
                .CountAsync(o => o.Status == OfferStatus.Approved);
        }

        /// <inheritdoc/>
        public async Task<int> CountFulfilledOffersAsync()
        {
            return await _context.Offers
                .CountAsync(o => o.Status == OfferStatus.Fulfilled);
        }

        /// <inheritdoc/>
        public async Task<int> SumFulfilledOffersQuantityAsync()
        {
            return await _context.Offers
                .Where(o => o.Status == OfferStatus.Fulfilled)
                .SumAsync(o => o.Quantity);
        }

        /// <summary>
        /// Builds the base approved query with optional filters.
        /// Shared between data and count queries to keep filters consistent.
        /// </summary>
        private IQueryable<Offer> BuildApprovedQuery(
            string? category,
            string? city,
            string? governorate,
            string? search)
        {
            var query = _context.Offers
                .Include(o => o.DonorOrganization)
                    .ThenInclude(d => d.ApplicationUser)
                .Where(o => o.Status == OfferStatus.Approved);

            if (!string.IsNullOrWhiteSpace(category))
                query = query.Where(o => o.Category == category.Trim().ToLower());

            if (!string.IsNullOrWhiteSpace(city))
                query = query.Where(o => o.DonorOrganization.ApplicationUser.City == city.Trim().ToLower());

            if (!string.IsNullOrWhiteSpace(governorate))
                query = query.Where(o => o.DonorOrganization.ApplicationUser.Governorate == governorate.Trim().ToLower());

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(o => o.ProductName.Contains(search.Trim()));

            return query;
        }
        /// <inheritdoc/>
        public async Task<Offer?> GetApprovedOfferByIdAsync(Guid offerId)
        {
            return await _context.Offers
                .Include(o => o.DonorOrganization)
                    .ThenInclude(d => d.ApplicationUser)
                .FirstOrDefaultAsync(o =>
                    o.OfferId == offerId &&
                    o.Status == OfferStatus.Approved);
        }
    }
}