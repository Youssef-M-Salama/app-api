using App.Core.DTOs.Response;
using App.Core.DTOs.ResultPattern;

namespace App.Core.ServiceContracts
{
    /// <summary>
    /// Defnes service operations for Admins.
    /// </summary>
    public interface IAdminService
    {
        Task<ServiceResult<AdminDashboardResponseDTO>> GetDashboardStatisticsAsync();

        /// <summary>
        /// Retrieves lists of unverified charities and donor organizations.
        /// </summary>
        Task<ServiceResult<PendingVerificationsResponseDTO>> GetPendingVerificationsAsync();
        
        Task<ServiceResult<object>> VerifyUserAsync(App.Core.DTOs.Request.ActionUserRequestDTO request);
        Task<ServiceResult<object>> RejectUserAsync(App.Core.DTOs.Request.ActionUserRequestDTO request);
        
        Task<ServiceResult<IEnumerable<App.Core.DTOs.Response.CharityNeedResponseDTO>>> GetPendingCharityNeedsAsync(App.Core.DTOs.Request.PendingRequestsFilterDTO query);
        
        Task<ServiceResult<object>> ApproveCharityNeedAsync(App.Core.DTOs.Request.ActionCharityNeedRequestDTO request);
        Task<ServiceResult<object>> RejectCharityNeedAsync(App.Core.DTOs.Request.ActionCharityNeedRequestDTO request);

        Task<ServiceResult<IEnumerable<App.Core.DTOs.Response.OfferResponseDTO>>> GetPendingOffersAsync(App.Core.DTOs.Request.PendingRequestsFilterDTO query);

        Task<ServiceResult<object>> ApproveOfferAsync(App.Core.DTOs.Request.ActionOfferRequestDTO request);
        Task<ServiceResult<object>> RejectOfferAsync(App.Core.DTOs.Request.ActionOfferRequestDTO request);
        
        Task<ServiceResult<IEnumerable<App.Core.DTOs.Response.UserResponseDTO>>> GetAllUsersAsync(App.Core.DTOs.Request.UsersFilterRequestDTO query);
        
        Task<ServiceResult<object>> DeactivateUserAsync(App.Core.DTOs.Request.ActionUserRequestDTO request);
        Task<ServiceResult<object>> ActivateUserAsync(App.Core.DTOs.Request.ActionUserRequestDTO request);
    }
}
