using App.Core.Domain.Entities;
using App.Core.Domain.Enums;
using App.Core.Domain.RepositoryContracts;
using App.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace App.Infrastructure.Repository
{
    public class CharityRepository : ICharityRepository
    {
        private readonly ApplicationDbContext _context;

        public CharityRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        /// <inheritdoc/>
        public async Task<int> CountTotalCharitiesAsync()
        {
            return await _context.Charities
                .CountAsync(c => c.VerificationState == VerificationState.Verified && c.IsActive);
        }

        /// <inheritdoc/>
        public async Task AddAsync(Charity charity)
        {
            _context.Charities.Add(charity);
            await _context.SaveChangesAsync();
        }

        public async Task<VerificationState?> GetVerificationStateByUserIdAsync(Guid userId)
        {
            var charity = await _context.Charities.FirstOrDefaultAsync(c => c.UserId == userId);
            return charity?.VerificationState;
        }

        public async Task<Charity?> GetByUserIdAsync(Guid userId)
        {
            return await _context.Charities.FirstOrDefaultAsync(c => c.UserId == userId);
        }
    }
}
