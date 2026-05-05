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
            ProductCategory? category,
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
            ProductCategory? category,
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
        public async Task<decimal> SumFulfilledOffersQuantityAsync()
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
            ProductCategory? category,
            string? city,
            string? governorate,
            string? search)
        {
            var query = _context.Offers
                .Include(o => o.DonorOrganization)
                    .ThenInclude(d => d.ApplicationUser)
                .Where(o => o.Status == OfferStatus.Approved);

            if (category.HasValue)
                query = query.Where(o => o.Category == category.Value);

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

        public async Task<(int Total, int Pending, int Approved, int Rejected, int Fulfilled, int Expired)> GetOfferCountsByDonorOrganizationIdAsync(Guid DonorOrganizationId)
        {
            var counts = await _context.Offers
                .Where(o => o.DonorOrganizationId == DonorOrganizationId)
                .GroupBy(o => 1) // Group all offers together to get aggregate counts
                .Select(g => new
                {
                    Total = g.Count(),
                    Pending = g.Count(o => o.Status == OfferStatus.Pending),
                    Approved = g.Count(o => o.Status == OfferStatus.Approved),
                    Rejected = g.Count(o => o.Status == OfferStatus.Rejected),
                    Fulfilled = g.Count(o => o.Status == OfferStatus.Fulfilled),
                    Expired = g.Count(o => o.Status == OfferStatus.Expired)
                })
                .FirstOrDefaultAsync();
            return counts is null ? (0, 0, 0, 0, 0, 0) : (counts.Total, counts.Pending, counts.Approved, counts.Rejected, counts.Fulfilled, counts.Expired);

        }

        public async Task<Offer> CreateAsync(Offer offer)
        {
            await _context.Offers.AddAsync(offer);
            await _context.SaveChangesAsync();
            return offer;
        }

        public async Task<IEnumerable<Offer>> GetByDonorOrganizationIdAsync(
            Guid donorId,
            OfferStatus? status,
            int page,
            int pageSize)
        {
            var query = _context.Offers.Where(o => o.DonorOrganizationId == donorId);
            if (status.HasValue)
            {
                query = query.Where(o => o.Status == status.Value);
            }
            return await query
                .Include(o => o.DonorOrganization)
                    .ThenInclude(d => d.ApplicationUser)
                .OrderByDescending(o => o.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> CountByDonorOrganizationIdAsync(Guid donorId, OfferStatus? status)
        {
            var query = _context.Offers.Where(o => o.DonorOrganizationId == donorId);
            if (status.HasValue)
            {
                query = query.Where(o => o.Status == status.Value);
            }
            return await query.CountAsync();
        }

        public async Task<Offer?> GetByIdWithDonorAsync(Guid offerId)
        {
            return await _context.Offers
                .Include(o => o.DonorOrganization)
                    .ThenInclude(d => d.ApplicationUser)
                .FirstOrDefaultAsync(o => o.OfferId == offerId);
        }

        public async Task UpdateAsync(Offer offer)
        {
            _context.Offers.Update(offer);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Offer offer)
        {
            _context.Offers.Remove(offer);
            await _context.SaveChangesAsync();
        }
    }
}