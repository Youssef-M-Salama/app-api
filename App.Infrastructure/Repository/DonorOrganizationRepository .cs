using App.Core.Domain.Entities;
using App.Core.Domain.Enums;
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
                .CountAsync(d => d.VerificationState == VerificationState.Verified && d.IsActive);
        }

        /// <inheritdoc/>
        public async Task AddAsync(DonorOrganization donor)
        {
            _context.DonorOrganizations.Add(donor);
            await _context.SaveChangesAsync();
        }

        public async Task<VerificationState?> GetVerificationStateByUserIdAsync(Guid userId)
        {
            var donor = await _context.DonorOrganizations.FirstOrDefaultAsync(d => d.UserId == userId);
            return donor?.VerificationState;
        }

        public async Task<DonorOrganization?> GetByUserIdAsync(Guid userId)
        {
            return await _context.DonorOrganizations.FirstOrDefaultAsync(d => d.UserId == userId);
        }
    }
}