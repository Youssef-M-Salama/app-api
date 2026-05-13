using App.Core.DTOs.Response;
using App.Core.DTOs.ResultPattern;

namespace App.Core.ServiceContracts
{
    /// <summary>
    /// Defines service operations for Admin-level management.
    /// </summary>
    public interface IAdminService
    {
        /// <summary>
        /// Retrieves aggregated dashboard statistics for the admin dashboard.
        /// </summary>
        Task<ServiceResult<AdminDashboardResponseDTO>> GetDashboardStatisticsAsync();

        /// <summary>
        /// Retrieves lists of unverified charities and donor organizations (Pending or InReview).
        /// </summary>
        Task<ServiceResult<PendingVerificationsResponseDTO>> GetPendingVerificationsAsync();
        
        /// <summary>
        /// Verifies a user account, setting its state to Verified (2).
        /// </summary>
        Task<ServiceResult<object>> VerifyUserAsync(App.Core.DTOs.Request.ActionUserRequestDTO request);

        /// <summary>
        /// Marks a user account as InReview (1).
        /// </summary>
        Task<ServiceResult<object>> MarkAsInReviewAsync(App.Core.DTOs.Request.ActionUserRequestDTO request);

        /// <summary>
        /// Rejects a user account, setting its state to Rejected (3).
        /// </summary>
        Task<ServiceResult<object>> RejectUserAsync(App.Core.DTOs.Request.ActionUserRequestDTO request);
        
        /// <summary>
        /// Retrieves a paginated list of charity needs that are awaiting approval (Pending status).
        /// </summary>
        Task<ServiceResult<IEnumerable<App.Core.DTOs.Response.CharityNeedResponseDTO>>> GetPendingCharityNeedsAsync(App.Core.DTOs.Request.PendingRequestsFilterDTO query);
        
        /// <summary>
        /// Approves a charity need, making it visible to donors.
        /// </summary>
        Task<ServiceResult<object>> ApproveCharityNeedAsync(App.Core.DTOs.Request.ActionCharityNeedRequestDTO request);

        /// <summary>
        /// Rejects a charity need.
        /// </summary>
        Task<ServiceResult<object>> RejectCharityNeedAsync(App.Core.DTOs.Request.ActionCharityNeedRequestDTO request);

        /// <summary>
        /// Retrieves a paginated list of offers that are awaiting approval (Pending status).
        /// </summary>
        Task<ServiceResult<IEnumerable<App.Core.DTOs.Response.OfferResponseDTO>>> GetPendingOffersAsync(App.Core.DTOs.Request.PendingRequestsFilterDTO query);

        /// <summary>
        /// Approves a donor offer, making it visible to charities.
        /// </summary>
        Task<ServiceResult<object>> ApproveOfferAsync(App.Core.DTOs.Request.ActionOfferRequestDTO request);

        /// <summary>
        /// Rejects a donor offer.
        /// </summary>
        Task<ServiceResult<object>> RejectOfferAsync(App.Core.DTOs.Request.ActionOfferRequestDTO request);
        
        /// <summary>
        /// Retrieves all users with optional filtering by role and active status.
        /// </summary>
        Task<ServiceResult<IEnumerable<App.Core.DTOs.Response.UserResponseDTO>>> GetAllUsersAsync(App.Core.DTOs.Request.UsersFilterRequestDTO query);
        
        /// <summary>
        /// Deactivates a user account.
        /// </summary>
        Task<ServiceResult<object>> DeactivateUserAsync(App.Core.DTOs.Request.ActionUserRequestDTO request);

        /// <summary>
        /// Activates a previously deactivated user account.
        /// </summary>
        Task<ServiceResult<object>> ActivateUserAsync(App.Core.DTOs.Request.ActionUserRequestDTO request);
    }
}
