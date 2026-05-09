using App.Core.Domain.Entities;
using App.Core.Domain.RepositoryContracts;
using App.Core.Enums;
using App.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace App.Infrastructure.Repository
{
    public class NeedApplicationRepository : INeedApplicationRepository
    {
        private readonly ApplicationDbContext _context;

        public NeedApplicationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================================================
        // CHARITY — applications received on my needs
        // =========================================================

        /// <inheritdoc/>
        public async Task<IEnumerable<NeedApplication>> GetReceivedByCharityIdAsync(
            Guid charityId,
            int page,
            int pageSize)
        {
            return await _context.NeedApplications
                .Include(na => na.CharityNeed)
                .Include(na => na.DonorOrganization)
                    .ThenInclude(d => d.ApplicationUser)
                .Where(na => na.CharityNeed.CharityId == charityId)
                .OrderByDescending(na => na.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<int> CountReceivedByCharityIdAsync(Guid charityId)
        {
            return await _context.NeedApplications
                .Where(na => na.CharityNeed.CharityId == charityId)
                .CountAsync();
        }

        /// <inheritdoc/>
        public async Task<NeedApplication?> GetByIdAsync(Guid needApplicationId)
        {
            return await _context.NeedApplications
                .Include(na => na.CharityNeed)
                .Include(na => na.DonorOrganization)    
                .ThenInclude(d=>d.ApplicationUser)
                .FirstOrDefaultAsync(na => na.NeedApplicationId == needApplicationId);
        }

        /// <inheritdoc/>
        public async Task<NeedApplication> UpdateAsync(NeedApplication application)
        {
            _context.NeedApplications.Update(application);
            await _context.SaveChangesAsync();
            return application;
        }

        /// <inheritdoc/>
        public async Task<NeedApplication> CreateAsync(NeedApplication application)
        {
            await _context.NeedApplications.AddAsync(application);
            await _context.SaveChangesAsync();
            return application;
        }

        /// <inheritdoc/>
        public async Task<bool> ExistsAsync(Guid donorOrganizationId, Guid charityNeedId)
        {
            return await _context.NeedApplications.AnyAsync(na =>
                na.DonorOrganizationId == donorOrganizationId &&
                na.CharityNeedId == charityNeedId);
        }

        // =========================================================
        // CHARITY DASHBOARD
        // =========================================================

        /// <inheritdoc/>
        public async Task<(int Total, int Pending, int Accepted, int Rejected)>
            GetReceivedCountsByCharityIdAsync(Guid charityId)
        {
            var counts = await _context.NeedApplications
                .Where(na => na.CharityNeed.CharityId == charityId)
                .GroupBy(_ => 1)
                .Select(g => new
                {
                    Total = g.Count(),
                    Pending = g.Count(na => na.Status == ApplicationStatus.Pending),
                    Accepted = g.Count(na => na.Status == ApplicationStatus.Accepted),
                    Rejected = g.Count(na => na.Status == ApplicationStatus.Rejected)
                })
                .FirstOrDefaultAsync();

            return counts is null
                ? (0, 0, 0, 0)
                : (counts.Total, counts.Pending, counts.Accepted, counts.Rejected);
        }

        public async Task<(int Total, int Pending, int Accepted, int Rejected)> GetSentCountsByDonorOrganizationIdAsync(Guid donorOrganizationId)
        {
            var counts = await _context.NeedApplications
                .Where(na => na.DonorOrganizationId == donorOrganizationId)
                .GroupBy(_ => 1)
                .Select(g => new
                {
                    Total = g.Count(),
                    Pending = g.Count(na => na.Status == ApplicationStatus.Pending),
                    Accepted = g.Count(na => na.Status == ApplicationStatus.Accepted),
                    Rejected = g.Count(na => na.Status == ApplicationStatus.Rejected)
                })
                .FirstOrDefaultAsync();
            return counts is null
                ? (0, 0, 0, 0)
                : (counts.Total, counts.Pending, counts.Accepted, counts.Rejected);

        }

        public async Task<IEnumerable<NeedApplication>> GetSentByDonorOrganizationIdAsync(Guid donorOrganizationId, int page, int pageSize)
        {
            return await _context.NeedApplications
                .Include(na => na.CharityNeed)
                    .ThenInclude(cn => cn.Charity)
                        .ThenInclude(c => c.ApplicationUser)
                .Where(na => na.DonorOrganizationId == donorOrganizationId)
                .OrderByDescending(na => na.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> CountSentByDonorOrganizationIdAsync(Guid donorOrganizationId)
        {
            return await _context.NeedApplications
                .CountAsync(na => na.DonorOrganizationId == donorOrganizationId);
        }

        public async Task DeleteAsync(NeedApplication application)
        {
            _context.NeedApplications.Remove(application);
            await _context.SaveChangesAsync();
        }
    }
}