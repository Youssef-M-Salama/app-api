using App.Core.Domain.Entities;
using App.Core.Domain.RepositoryContracts;
using App.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace App.Infrastructure.Repository
{
    public class DonorOrganizationRepository : IDonorOrganizationRepository
    {
        private readonly ApplicationDbContext _context;

        public DonorOrganizationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc/>
        public async Task<int> CountTotalDonorsAsync()
        {
            return await _context.DonorOrganizations
                .CountAsync(d => d.IsVerified && d.IsActive);
        }

        /// <inheritdoc/>
        public async Task AddAsync(DonorOrganization donor)
        {
            _context.DonorOrganizations.Add(donor);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsVerifiedByUserId(Guid userId)
        {
            var donor=_context.DonorOrganizations.FirstOrDefault(d=>d.UserId == userId);
            if (donor == null)
            {
                return false;
            }
            return donor.IsVerified;
        }

        public async Task<DonorOrganization?> GetByUserIdAsync(Guid userId)
        {
            return await _context.DonorOrganizations.FirstOrDefaultAsync(d => d.UserId == userId);
        }
    }
}