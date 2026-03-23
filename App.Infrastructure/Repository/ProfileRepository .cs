using App.Core.Domain.Entities;
using App.Core.Domain.RepositoryContracts;
using App.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace App.Infrastructure.Repository
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly ApplicationDbContext _context;

        public ProfileRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc/>
        public async Task<Charity?> GetCharityByUserIdAsync(Guid userId)
        {
            return await _context.Charities
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        /// <inheritdoc/>
        public async Task<DonorOrganization?> GetDonorOrganizationByUserIdAsync(Guid userId)
        {
            return await _context.DonorOrganizations
                .FirstOrDefaultAsync(d => d.UserId == userId);
        }
    }
}