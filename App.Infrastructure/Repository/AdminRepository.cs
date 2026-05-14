using App.Core.Domain.Enums;
using App.Core.Domain.RepositoryContracts;
using App.Core.Enums;
using App.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace App.Infrastructure.Repository
{
    public class AdminRepository : IAdminRepository
    {
        private readonly ApplicationDbContext _db;

        public AdminRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<(int PendingVerifications, int PendingCharityNeeds, int PendingOffers, int TotalUsers, int ActiveCharityNeeds, int ActiveOffers)> GetDashboardStatisticsAsync()
        {
            var pendingCharityVerifications = await _db.Charities.Include(c => c.ApplicationUser)
                .CountAsync(c => c.ApplicationUser.EmailConfirmed && 
                            (c.VerificationState == VerificationState.Pending || c.VerificationState == VerificationState.InReview));

            var pendingDonorVerifications = await _db.DonorOrganizations.Include(d => d.ApplicationUser)
                .CountAsync(d => d.ApplicationUser.EmailConfirmed && 
                            (d.VerificationState == VerificationState.Pending || d.VerificationState == VerificationState.InReview));

            var pendingCharityNeeds = await _db.CharityNeeds
                .CountAsync(cn => cn.Status == CharityNeedStatus.Pending);

            var pendingOffers = await _db.Offers
                .CountAsync(o => o.Status == OfferStatus.Pending);

            var totalUsers = await CountAllUsersAsync(null, null);

            var activeCharityNeeds = await _db.CharityNeeds
                .CountAsync(cn => cn.Status == CharityNeedStatus.Approved);

            var activeOffers = await _db.Offers
                .CountAsync(o => o.Status == OfferStatus.Approved);

            return (
                PendingVerifications: pendingCharityVerifications + pendingDonorVerifications,
                PendingCharityNeeds: pendingCharityNeeds,
                PendingOffers: pendingOffers,
                TotalUsers: totalUsers,
                ActiveCharityNeeds: activeCharityNeeds,
                ActiveOffers: activeOffers
            );
        }

        public async Task<(IEnumerable<App.Core.Domain.Entities.Charity> PendingCharities, IEnumerable<App.Core.Domain.Entities.DonorOrganization> PendingDonors)> GetPendingVerificationsAsync()
        {
            var pendingCharities = await _db.Charities
                .Include(c => c.ApplicationUser)
                .Where(c => c.VerificationState == VerificationState.Pending || c.VerificationState == VerificationState.InReview)
                .ToListAsync();

            var pendingDonors = await _db.DonorOrganizations
                .Include(d => d.ApplicationUser)
                .Where(d => d.VerificationState == VerificationState.Pending || d.VerificationState == VerificationState.InReview)
                .ToListAsync();

            return (pendingCharities, pendingDonors);
        }

        public async Task<(bool Success, string? Email, string? Username)> VerifyUserAsync(Guid userId)
        {
            var charity = await _db.Charities.FirstOrDefaultAsync(c => c.UserId == userId);
            var donor = await _db.DonorOrganizations.FirstOrDefaultAsync(d => d.UserId == userId);
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            
            if (user == null) return (false, null, null);

            if (user.EmailConfirmed == false) return (false, null, null);
            if (!user.VerifyMyAccount) return (false, null, null);
            if (charity != null)
            {
                charity.VerificationState = VerificationState.Verified;
                charity.IsActive = true;
                charity.UpdatedAt = DateTime.UtcNow;
            }
            if (donor != null)
            {
                donor.VerificationState = VerificationState.Verified;
                donor.IsActive = true;
                donor.UpdatedAt = DateTime.UtcNow;
            }
            
            user.IsActive = true;
            user.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return (true, user.Email, user.UserName);
        }

        public async Task<(bool Success, string? Email, string? Username)> MarkAsInReviewAsync(Guid userId)
        {
            var user = await _db.Users
                .Include(u => u.Charity)
                .Include(u => u.DonorOrganization)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return (false, null, null);
            if (!user.VerifyMyAccount) return (false, null, null);

            if (user.Charity != null)
            {
                if (user.Charity.VerificationState == VerificationState.Verified) return (false, null, null);
                user.Charity.VerificationState = VerificationState.InReview;
            }
            else if (user.DonorOrganization != null)
            {
                if (user.DonorOrganization.VerificationState == VerificationState.Verified) return (false, null, null);
                user.DonorOrganization.VerificationState = VerificationState.InReview;
            }

            await _db.SaveChangesAsync();
            return (true, user.Email, user.UserName);
        }

        public async Task<(bool Success, string? Email, string? Username)> RejectUserAsync(Guid userId)
        {
            var charity = await _db.Charities.FirstOrDefaultAsync(c => c.UserId == userId);
            var donor = await _db.DonorOrganizations.FirstOrDefaultAsync(d => d.UserId == userId);
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return (false, null, null);
            if (!user.VerifyMyAccount) return (false, null, null);

            var email = user.Email;
            var username = user.UserName;

            if (charity != null)
            {
                if (charity.VerificationState == VerificationState.Verified) return (false, null, null);
                charity.VerificationState = VerificationState.Rejected;
                charity.IsActive = false;
                charity.UpdatedAt = DateTime.UtcNow;
            }
            if (donor != null)
            {
                if (donor.VerificationState == VerificationState.Verified) return (false, null, null);
                donor.VerificationState = VerificationState.Rejected;
                donor.IsActive = false;
                donor.UpdatedAt = DateTime.UtcNow;
            }

            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return (true, email, username);
        }

        public async Task<IEnumerable<App.Core.Domain.Entities.CharityNeed>> GetPendingCharityNeedsAsync(int page, int pageSize)
        {
            return await _db.CharityNeeds
                .Include(cn => cn.Charity)
                .ThenInclude(c => c.ApplicationUser)
                .Where(cn => cn.Status == CharityNeedStatus.Pending)
                .OrderByDescending(cn => cn.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> CountPendingCharityNeedsAsync()
        {
            return await _db.CharityNeeds
                .CountAsync(cn => cn.Status == CharityNeedStatus.Pending);
        }

        public async Task<(bool Success, string? Email, string? Username, string? ProductName)> ApproveCharityNeedAsync(Guid charityNeedId)
        {
            var cn = await _db.CharityNeeds
                .Include(x => x.Charity)
                .ThenInclude(c => c.ApplicationUser)
                .FirstOrDefaultAsync(x => x.CharityNeedId == charityNeedId);

            if (cn == null || cn.Status != CharityNeedStatus.Pending)
                return (false, null, null, null);
            
            cn.Status = CharityNeedStatus.Approved;
            cn.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return (true, cn.Charity?.ApplicationUser?.Email, cn.Charity?.ApplicationUser?.UserName, cn.ProductName);
        }

        public async Task<(bool Success, string? Email, string? Username, string? ProductName)> RejectCharityNeedAsync(Guid charityNeedId)
        {
            var cn = await _db.CharityNeeds
                .Include(x => x.Charity)
                .ThenInclude(c => c.ApplicationUser)
                .FirstOrDefaultAsync(x => x.CharityNeedId == charityNeedId);

            if (cn == null || cn.Status != CharityNeedStatus.Pending)
                return (false, null, null, null);
            
            cn.Status = CharityNeedStatus.Rejected;
            cn.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return (true, cn.Charity?.ApplicationUser?.Email, cn.Charity?.ApplicationUser?.UserName, cn.ProductName);
        }

        public async Task<IEnumerable<App.Core.Domain.Entities.Offer>> GetPendingOffersAsync(int page, int pageSize)
        {
            return await _db.Offers
                .Include(o => o.DonorOrganization)
                .ThenInclude(d => d.ApplicationUser)
                .Where(o => o.Status == OfferStatus.Pending)
                .OrderByDescending(o => o.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> CountPendingOffersAsync()
        {
            return await _db.Offers
                .CountAsync(o => o.Status == OfferStatus.Pending);
        }

        public async Task<(bool Success, string? Email, string? Username, string? ProductName)> ApproveOfferAsync(Guid offerId)
        {
            var offer = await _db.Offers
                .Include(o => o.DonorOrganization)
                .ThenInclude(d => d.ApplicationUser)
                .FirstOrDefaultAsync(o => o.OfferId == offerId);

            if (offer == null || offer.Status != OfferStatus.Pending)
                return (false, null, null, null);

            offer.Status = OfferStatus.Approved;
            offer.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return (true, offer.DonorOrganization?.ApplicationUser?.Email, offer.DonorOrganization?.ApplicationUser?.UserName, offer.ProductName);
        }

        public async Task<(bool Success, string? Email, string? Username, string? ProductName)> RejectOfferAsync(Guid offerId)
        {
            var offer = await _db.Offers
                .Include(o => o.DonorOrganization)
                .ThenInclude(d => d.ApplicationUser)
                .FirstOrDefaultAsync(o => o.OfferId == offerId);

            if (offer == null || offer.Status != OfferStatus.Pending)
                return (false, null, null, null);

            offer.Status = OfferStatus.Rejected;
            offer.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return (true, offer.DonorOrganization?.ApplicationUser?.Email, offer.DonorOrganization?.ApplicationUser?.UserName, offer.ProductName);
        }

        public async Task<IEnumerable<App.Core.Domain.IdentityEntities.ApplicationUser>> GetAllUsersAsync(UserRole? role, bool? isActive, int page, int pageSize)
        {
            IQueryable<App.Core.Domain.IdentityEntities.ApplicationUser> query = _db.Users
                .Include(u => u.Charity)
                .Include(u => u.DonorOrganization)
                .Where(u => u.EmailConfirmed == true);

            // Exclude Rejected users (soft delete logic)
            query = query.Where(u => 
                (u.Charity == null || u.Charity.VerificationState != VerificationState.Rejected) &&
                (u.DonorOrganization == null || u.DonorOrganization.VerificationState != VerificationState.Rejected)
            );

            // Exclude Admin role by default
            var adminRole = await _db.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
            if (adminRole != null)
            {
                query = query.Where(u => !_db.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == adminRole.Id));
            }

            if (isActive.HasValue)
            {
                query = query.Where(u => u.IsActive == isActive.Value);
            }

            if (role.HasValue)
            {
                var roleName = role.Value.ToString();
                var roleEntity = await _db.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
                if (roleEntity != null)
                {
                    query = query.Where(u => _db.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == roleEntity.Id));
                }
            }
            return await query
                .Include(u => u.Charity)
                .Include(u => u.DonorOrganization)
                .OrderByDescending(u => u.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> CountAllUsersAsync(UserRole? role, bool? isActive)
        {
            IQueryable<App.Core.Domain.IdentityEntities.ApplicationUser> query = _db.Users;

            // Exclude unconfirmed emails by default
            query = query.Where(u => u.EmailConfirmed == true);

            // Exclude Rejected users
            query = query.Where(u => 
                (u.Charity == null || u.Charity.VerificationState != VerificationState.Rejected) &&
                (u.DonorOrganization == null || u.DonorOrganization.VerificationState != VerificationState.Rejected)
            );

            // Exclude Admin role by default
            var adminRole = await _db.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
            if (adminRole != null)
            {
                query = query.Where(u => !_db.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == adminRole.Id));
            }

            if (isActive.HasValue)
            {
                query = query.Where(u => u.IsActive == isActive.Value);
            }

            if (role.HasValue)
            {
                var roleName = role.Value.ToString();
                var roleEntity = await _db.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
                if (roleEntity != null)
                {
                    query = query.Where(u => _db.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == roleEntity.Id));
                }
            }

            return await query.CountAsync();
        }

        public async Task<bool> DeactivateUserAsync(Guid userId)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return false;

            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;

            var charity = await _db.Charities.FirstOrDefaultAsync(c => c.UserId == userId);
            if (charity != null) { charity.IsActive = false; charity.UpdatedAt = DateTime.UtcNow; }
            
            var donor = await _db.DonorOrganizations.FirstOrDefaultAsync(d => d.UserId == userId);
            if (donor != null) { donor.IsActive = false; donor.UpdatedAt = DateTime.UtcNow; }

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ActivateUserAsync(Guid userId)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return false;

            user.IsActive = true;
            user.UpdatedAt = DateTime.UtcNow;

            var charity = await _db.Charities.FirstOrDefaultAsync(c => c.UserId == userId);
            if (charity != null) { charity.IsActive = true; charity.UpdatedAt = DateTime.UtcNow; }
            
            var donor = await _db.DonorOrganizations.FirstOrDefaultAsync(d => d.UserId == userId);
            if (donor != null) { donor.IsActive = true; donor.UpdatedAt = DateTime.UtcNow; }

            await _db.SaveChangesAsync();
            return true;
        }
    }
}
