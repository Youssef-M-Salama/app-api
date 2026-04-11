using App.Core.Enums;

namespace App.Core.Domain.RepositoryContracts
{
    /// <summary>
    /// Repository contract for Admin-specific data access.
    /// </summary>
    public interface IAdminRepository
    {
        /// <summary>
        /// Retrieves aggregated dashboard statistics for the admin dashboard.
        /// </summary>
        Task<(int PendingVerifications, int PendingCharityNeeds, int PendingOffers, int TotalUsers, int ActiveCharityNeeds, int ActiveOffers)> GetDashboardStatisticsAsync();

        /// <summary>
        /// Retrieves lists of unverified charities and donor organizations.
        /// </summary>
        Task<(IEnumerable<App.Core.Domain.Entities.Charity> PendingCharities, IEnumerable<App.Core.Domain.Entities.DonorOrganization> PendingDonors)> GetPendingVerificationsAsync();

        /// <summary>
        /// Verifies a user account. Returns user info for email notification.
        /// </summary>
        Task<(bool Success, string? Email, string? Username)> VerifyUserAsync(Guid userId);

        /// <summary>
        /// Rejects and removes a user account. Returns user info for email notification.
        /// </summary>
        Task<(bool Success, string? Email, string? Username)> RejectUserAsync(Guid userId);

        Task<IEnumerable<App.Core.Domain.Entities.CharityNeed>> GetPendingCharityNeedsAsync(int page, int pageSize);
        Task<int> CountPendingCharityNeedsAsync();
        
        /// <summary>
        /// Approves a charity need. Returns charity owner info for email notification.
        /// </summary>
        Task<(bool Success, string? Email, string? Username, string? ProductName)> ApproveCharityNeedAsync(Guid charityNeedId);

        /// <summary>
        /// Rejects a charity need. Returns charity owner info for email notification.
        /// </summary>
        Task<(bool Success, string? Email, string? Username, string? ProductName)> RejectCharityNeedAsync(Guid charityNeedId);

        Task<IEnumerable<App.Core.Domain.IdentityEntities.ApplicationUser>> GetAllUsersAsync(UserRole? role, bool? isActive, int page, int pageSize);
        Task<int> CountAllUsersAsync(UserRole? role, bool? isActive);
        
        Task<IEnumerable<App.Core.Domain.Entities.Offer>> GetPendingOffersAsync(int page, int pageSize);
        Task<int> CountPendingOffersAsync();

        /// <summary>
        /// Approves an offer. Returns donor owner info for email notification.
        /// </summary>
        Task<(bool Success, string? Email, string? Username, string? ProductName)> ApproveOfferAsync(Guid offerId);

        /// <summary>
        /// Rejects an offer. Returns donor owner info for email notification.
        /// </summary>
        Task<(bool Success, string? Email, string? Username, string? ProductName)> RejectOfferAsync(Guid offerId);

        Task<bool> DeactivateUserAsync(Guid userId);
        Task<bool> ActivateUserAsync(Guid userId);
    }
}
