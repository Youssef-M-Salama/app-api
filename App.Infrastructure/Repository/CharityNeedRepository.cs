using App.Core.Domain.Entities;
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
                .CountAsync(cn => cn.Status == CharityNeedStatus.Approved);
        }

        /// <inheritdoc/>
        public async Task<int> CountApprovedCharityNeedsAsync(
            string? category,
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
                .CountAsync(cn => cn.Status == CharityNeedStatus.Fulfilled);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<CharityNeed>> GetApprovedCharityNeedsAsync(
            string? category,
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
        public async Task<int> SumFulfilledCharityNeedsQuantityAsync()
        {
            return await _context.CharityNeeds
                .Where(cn => cn.Status == CharityNeedStatus.Fulfilled)
                .SumAsync(cn => cn.Quantity);
        }

        /// <summary>
        /// Builds the base approved query with optional filters.
        /// Shared between data and count queries to keep filters consistent.
        /// </summary>
        private IQueryable<CharityNeed> BuildApprovedQuery(
            string? category,
            string? city,
            string? governorate,
            string? search)
        {
            var query = _context.CharityNeeds
                .Include(cn => cn.Charity)
                    .ThenInclude(c => c.ApplicationUser)
                .Where(cn => cn.Status == CharityNeedStatus.Approved);

            if (!string.IsNullOrWhiteSpace(category))
                query = query.Where(cn => cn.Category == category.Trim().ToLower());

            if (!string.IsNullOrWhiteSpace(city))
                query = query.Where(cn => cn.Charity.ApplicationUser.City == city.Trim().ToLower());

            if (!string.IsNullOrWhiteSpace(governorate))
                query = query.Where(cn => cn.Charity.ApplicationUser.Governorate == governorate.Trim().ToLower());

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(cn => cn.ProductName.Contains(search.Trim()));

            return query;
        }
        /// <inheritdoc/>
        public async Task<CharityNeed?> GetApprovedCharityNeedByIdAsync(Guid charityNeedId)
        {
            return await _context.CharityNeeds
                .Include(cn => cn.Charity)
                    .ThenInclude(c => c.ApplicationUser)
                .FirstOrDefaultAsync(cn =>
                    cn.CharityNeedId == charityNeedId &&
                    cn.Status == CharityNeedStatus.Approved);
        }
    }
}