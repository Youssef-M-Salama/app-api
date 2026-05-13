using App.Core.Domain.Entities;
using App.Core.Domain.Enums;
using App.Core.Domain.RepositoryContracts;
using App.Core.Enums;
using App.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace App.Infrastructure.Repository
{
    public class CharityNeedRepository : ICharityNeedRepository
    {
        private readonly ApplicationDbContext _context;

        public CharityNeedRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc/>
        public async Task<int> CountActiveCharityNeedsAsync()
        {
            return await _context.CharityNeeds
                .Include(cn => cn.Charity)
                .CountAsync(cn => cn.Status == CharityNeedStatus.Approved && 
                                 cn.Charity.VerificationState == VerificationState.Verified && 
                                 cn.Charity.IsActive);
        }

        /// <inheritdoc/>
        public async Task<int> CountApprovedCharityNeedsAsync(
            ProductCategory? category,
            string? city,
            string? governorate,
            string? search)
        {
            return await BuildApprovedQuery(category, city, governorate, search).CountAsync();
        }

        /// <inheritdoc/>
        public async Task<int> CountFulfilledCharityNeedsAsync()
        {
            return await _context.CharityNeeds
                .Include(cn => cn.Charity)
                .CountAsync(cn => cn.Status == CharityNeedStatus.Fulfilled && 
                                 cn.Charity.VerificationState == VerificationState.Verified && 
                                 cn.Charity.IsActive);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<CharityNeed>> GetApprovedCharityNeedsAsync(
            ProductCategory? category,
            string? city,
            string? governorate,
            string? search,
            int page,
            int pageSize)
        {
            return await BuildApprovedQuery(category, city, governorate, search)
                .OrderByDescending(cn => cn.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<decimal> SumFulfilledCharityNeedsQuantityAsync()
        {
            return await _context.CharityNeeds
                .Include(cn => cn.Charity)
                .Where(cn => cn.Status == CharityNeedStatus.Fulfilled && 
                            cn.Charity.VerificationState == VerificationState.Verified && 
                            cn.Charity.IsActive)
                .SumAsync(cn => cn.Quantity);
        }

        /// <inheritdoc/>
        public async Task<CharityNeed?> GetApprovedCharityNeedByIdAsync(Guid charityNeedId)
        {
            return await _context.CharityNeeds
                .Include(cn => cn.Charity)
                    .ThenInclude(c => c.ApplicationUser)
                .FirstOrDefaultAsync(cn =>
                    cn.CharityNeedId == charityNeedId &&
                    cn.Status == CharityNeedStatus.Approved &&
                    cn.Charity.VerificationState == VerificationState.Verified &&
                    cn.Charity.IsActive);
        }


        /// <inheritdoc/>
        public async Task<CharityNeed?> GetByIdWithCharityAsync(Guid charityNeedId)
        {
            return await _context.CharityNeeds
                .Include(cn => cn.Charity)
                    .ThenInclude(c => c.ApplicationUser)
                .FirstOrDefaultAsync(cn => cn.CharityNeedId == charityNeedId);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<CharityNeed>> GetByCharityIdAsync(
            Guid charityId,
            CharityNeedStatus? status,
            int page,
            int pageSize)
        {
            var query = _context.CharityNeeds
                .Where(cn => cn.CharityId == charityId);

            if (status.HasValue)
            {
                query = query.Where(cn => cn.Status == status.Value);
            }

            return await query
                .Include(cn => cn.Charity)
                    .ThenInclude(c => c.ApplicationUser)
                .OrderByDescending(cn => cn.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<int> CountByCharityIdAsync(Guid charityId, CharityNeedStatus? status)
        {
            var query = _context.CharityNeeds
                .Where(cn => cn.CharityId == charityId);

            if (status.HasValue)
            {
                query = query.Where(cn => cn.Status == status.Value);
            }

            return await query.CountAsync();
        }

        /// <inheritdoc/>
        public async Task<CharityNeed> CreateAsync(CharityNeed need)
        {
            await _context.CharityNeeds.AddAsync(need);
            await _context.SaveChangesAsync();
            return need;
        }

        /// <inheritdoc/>
        public async Task<CharityNeed> UpdateAsync(CharityNeed need)
        {
            _context.CharityNeeds.Update(need);
            await _context.SaveChangesAsync();
            return need;
        }

        /// <inheritdoc/>
        public async Task DeleteAsync(CharityNeed need)
        {
            _context.CharityNeeds.Remove(need);
            await _context.SaveChangesAsync();
        }

        // =========================================================
        // CHARITY DASHBOARD
        // =========================================================

        /// <inheritdoc/>
        public async Task<(int Total, int Pending, int Approved, int Rejected, int Fulfilled)>
            GetNeedCountsByCharityIdAsync(Guid charityId)
        {
            var counts = await _context.CharityNeeds
                .Where(cn => cn.CharityId == charityId)
                .GroupBy(_ => 1)
                .Select(g => new
                {
                    Total = g.Count(),
                    Pending = g.Count(cn => cn.Status == CharityNeedStatus.Pending),
                    Approved = g.Count(cn => cn.Status == CharityNeedStatus.Approved),
                    Rejected = g.Count(cn => cn.Status == CharityNeedStatus.Rejected),
                    Fulfilled = g.Count(cn => cn.Status == CharityNeedStatus.Fulfilled)
                })
                .FirstOrDefaultAsync();

            // If the charity has no needs yet, counts will be null
            return counts is null
                ? (0, 0, 0, 0, 0)
                : (counts.Total, counts.Pending, counts.Approved, counts.Rejected, counts.Fulfilled);
        }

        // =========================================================
        // PRIVATE HELPERS
        // =========================================================

        /// <summary>
        /// Builds the base approved query with optional filters.
        /// Shared between data and count queries to keep filters consistent.
        /// </summary>
        private IQueryable<CharityNeed> BuildApprovedQuery(
            ProductCategory? category,
            string? city,
            string? governorate,
            string? search)
        {
            var query = _context.CharityNeeds
                .Include(cn => cn.Charity)
                    .ThenInclude(c => c.ApplicationUser)
                .Where(cn => cn.Status == CharityNeedStatus.Approved && 
                            cn.Charity.VerificationState == VerificationState.Verified && 
                            cn.Charity.IsActive);

            if (category.HasValue)
                query = query.Where(cn => cn.Category == category.Value);

            if (!string.IsNullOrWhiteSpace(city))
                query = query.Where(cn => cn.Charity.ApplicationUser.City == city.Trim().ToLower());

            if (!string.IsNullOrWhiteSpace(governorate))
                query = query.Where(cn => cn.Charity.ApplicationUser.Governorate == governorate.Trim().ToLower());

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(cn => cn.ProductName.Contains(search.Trim()));

            return query;
        }
    }
}