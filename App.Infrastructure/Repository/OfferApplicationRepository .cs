using App.Core.Domain.Entities;
using App.Core.Domain.RepositoryContracts;
using App.Core.Enums;
using App.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace App.Infrastructure.Repository
{
    public class OfferApplicationRepository : IOfferApplicationRepository
    {
        private readonly ApplicationDbContext _context;

        public OfferApplicationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================================================
        // CHARITY — applications sent to offers
        // =========================================================

        /// <inheritdoc/>
        public async Task<IEnumerable<OfferApplication>> GetSentByCharityIdAsync(
            Guid charityId,
            int page,
            int pageSize)
        {
            return await _context.OfferApplications
                .Include(oa => oa.Offer)
                    .ThenInclude(o => o.DonorOrganization)
                .Where(oa => oa.CharityId == charityId)
                .OrderByDescending(oa => oa.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<int> CountSentByCharityIdAsync(Guid charityId)
        {
            return await _context.OfferApplications
                .Where(oa => oa.CharityId == charityId)
                .CountAsync();
        }

        /// <inheritdoc/>
        public async Task<OfferApplication?> GetByIdAsync(Guid offerApplicationId)
        {
            return await _context.OfferApplications
                .Include(oa => oa.Offer)
                    .ThenInclude(o => o.DonorOrganization)
                .FirstOrDefaultAsync(oa => oa.OfferApplicationId == offerApplicationId);
        }

        /// <inheritdoc/>
        public async Task<bool> ExistsAsync(Guid charityId, Guid offerId)
        {
            return await _context.OfferApplications
                .AnyAsync(oa => oa.CharityId == charityId && oa.OfferId == offerId);
        }

        /// <inheritdoc/>
        public async Task<OfferApplication> CreateAsync(OfferApplication application)
        {
            await _context.OfferApplications.AddAsync(application);
            await _context.SaveChangesAsync();
            return application;
        }

        /// <inheritdoc/>
        public async Task DeleteAsync(OfferApplication application)
        {
            _context.OfferApplications.Remove(application);
            await _context.SaveChangesAsync();
        }

        // =========================================================
        // CHARITY DASHBOARD
        // =========================================================

        /// <inheritdoc/>
        public async Task<(int Total, int Pending, int Accepted, int Rejected)>
            GetSentCountsByCharityIdAsync(Guid charityId)
        {
            var counts = await _context.OfferApplications
                .Where(oa => oa.CharityId == charityId)
                .GroupBy(_ => 1)
                .Select(g => new
                {
                    Total = g.Count(),
                    Pending = g.Count(oa => oa.Status == ApplicationStatus.Pending),
                    Accepted = g.Count(oa => oa.Status == ApplicationStatus.Accepted),
                    Rejected = g.Count(oa => oa.Status == ApplicationStatus.Rejected)
                })
                .FirstOrDefaultAsync();

            return counts is null
                ? (0, 0, 0, 0)
                : (counts.Total, counts.Pending, counts.Accepted, counts.Rejected);
        }
        // =========================================================
        // Offer DASHBOARD
        // =========================================================

        public async Task<(int Total, int Pending, int Accepted, int Rejected)> GetReceivedCountsByDonorOrganizationIdAsync(Guid donorOrganizationId)
        {
            var counts = _context.OfferApplications
                .Where(oa => oa.Offer.DonorOrganizationId == donorOrganizationId)
                .GroupBy(_ => 1)
                .Select(g => new
                {
                    Total = g.Count(),
                    Pending = g.Count(oa => oa.Status == ApplicationStatus.Pending),
                    Accepted = g.Count(oa => oa.Status == ApplicationStatus.Accepted),
                    Rejected = g.Count(oa => oa.Status == ApplicationStatus.Rejected)
                })
                .FirstOrDefault();  
            return counts is null
                ? (0, 0, 0, 0)
                : (counts.Total, counts.Pending, counts.Accepted, counts.Rejected);
        }

    
        public async Task<IEnumerable<OfferApplication>> GetReceivedByDonorOrganizationIdAsync(
            Guid donorId,
            int page,
            int pageSize)
        {
            return await _context.OfferApplications
                .Include(oa => oa.Offer)
                .Include(oa => oa.Charity)
                .Where(oa => oa.Offer.DonorOrganizationId == donorId)
                .OrderByDescending(oa => oa.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task UpdateAsync(OfferApplication application)
        {
            _context.OfferApplications.Update(application);
            await _context.SaveChangesAsync();
        }
}
}