using App.Core.Domain.Entities;
using App.Core.Domain.RepositoryContracts;
using App.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

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
                .CountAsync(c => c.IsVerified && c.IsActive);
        }

        /// <inheritdoc/>
        public async Task AddAsync(Charity charity)
        {
            _context.Charities.Add(charity);
            await _context.SaveChangesAsync();
        }
    }
}
