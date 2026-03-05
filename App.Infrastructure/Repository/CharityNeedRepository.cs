using App.Core.Domain.Entities;
using App.Core.RepositoryContracts;
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
        public async Task<int> CountApprovedRequestsAsync(string? category, string? search)
        {
            return await BuildApprovedQuery(category, search).CountAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<CharityNeed>> GetApprovedRequestsAsync(
            string? category,
            string? search,
            int page,
            int pageSize)
        {
            return await BuildApprovedQuery(category, search)
                .OrderByDescending(cn => cn.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        /// <summary>
        /// Builds the base query for approved charity needs with optional filters.
        /// Shared between data and count queries to keep filters consistent.
        /// </summary>
        private IQueryable<CharityNeed> BuildApprovedQuery(
            string? category,
            string? search)
        {
            var query = _context.CharityNeeds
                .Include(cn => cn.Charity)
                    .ThenInclude(c => c.ApplicationUser)
                .Where(cn => cn.Status == "approved");

            if (!string.IsNullOrWhiteSpace(category))
                query = query.Where(cn => cn.Category == category.Trim().ToLower());

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(cn => cn.ProductName.Contains(search.Trim()));

            return query;
        }

    }
}
