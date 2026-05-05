using App.Core.DTOs.Request;
using App.Core.ServiceContracts;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers.V1
{
    /// <summary>
    /// Admin endpoints for managing users, verifications, charity needs, and offers.
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : CustomControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        // =========================================================
        // DASHBOARD
        // =========================================================

        /// <summary>
        /// Retrieves aggregated dashboard statistics for Admin.
        /// Includes counts of total users, pending verifications, pending charity needs, and pending offers.
        /// </summary>
        /// <response code="200">Statistics retrieved successfully.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="403">Forbidden access, not an Admin.</response>
        /// <response code="500">Unexpected server error.</response>
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboardStatistics()
        {
            var result = await _adminService.GetDashboardStatisticsAsync();
            return StatusCode((int)result.StatusCode, result.Response);
        }

        // =========================================================
        // USER VERIFICATIONS
        // =========================================================

        /// <summary>
        /// Retrieves lists of unverified charities and donor organizations.
        /// Returns users with ApplicationStatus Pending (0).
        /// </summary>
        /// <response code="200">Pending verifications retrieved successfully.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="403">Forbidden access, not an Admin.</response>
        /// <response code="500">Unexpected server error.</response>
        /// <remarks>
        /// ApplicationStatus: 0 (Pending), 1 (Accepted), 2 (Rejected)
        /// 
        /// UserRole: 0 (Charity), 1 (DonorOrganization), 2 (Admin)
        /// </remarks>
        [HttpGet("verifications/pending")]
        public async Task<IActionResult> GetPendingVerifications()
        {
            var result = await _adminService.GetPendingVerificationsAsync();
            return StatusCode((int)result.StatusCode, result.Response);
        }

        /// <summary>
        /// Verifies a user, changing their ApplicationStatus to Accepted.
        /// </summary>
        /// <param name="request">Request containing the user ID to verify.</param>
        /// <response code="200">User verified successfully.</response>
        /// <response code="400">Validation error.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="403">Forbidden access, not an Admin.</response>
        /// <response code="404">User not found.</response>
        /// <response code="500">Unexpected server error.</response>
        /// <remarks>
        /// Field Constraints:
        /// - UserId: Required, valid GUID
        /// </remarks>
        [HttpPost("verifications/verify")]
        public async Task<IActionResult> VerifyUser([FromBody] ActionUserRequestDTO request)
        {
            var result = await _adminService.VerifyUserAsync(request);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        // /// <summary>
        // /// Rejects a user registration, changing their ApplicationStatus to Rejected.
        // /// </summary>
        // /// <param name="request">Request containing the user ID to reject.</param>
        // /// <response code="200">User rejected successfully.</response>
        // /// <response code="400">Validation error.</response>
        // /// <response code="401">Unauthorized access.</response>
        // /// <response code="403">Forbidden access, not an Admin.</response>
        // /// <response code="404">User not found.</response>
        // /// <response code="500">Unexpected server error.</response>
        // /// <remarks>
        // /// Field Constraints:
        // /// - UserId: Required, valid GUID
        // /// </remarks>
        // [HttpPost("verifications/reject")]
        // public async Task<IActionResult> RejectUser([FromBody] ActionUserRequestDTO request)
        // {
        //     var result = await _adminService.RejectUserAsync(request);
        //     return StatusCode((int)result.StatusCode, result.Response);
        // }

        // =========================================================
        // CHARITY NEEDS MANAGEMENT
        // =========================================================

        /// <summary>
        /// Gets pending charity needs awaiting admin approval.
        /// Returns charity needs with status Pending (0).
        /// </summary>
        /// <param name="query">Filtering and pagination parameters.</param>
        /// <response code="200">Pending charity needs retrieved successfully.</response>
        /// <response code="400">Invalid pagination parameters.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="403">Forbidden access, not an Admin.</response>
        /// <response code="500">Unexpected server error.</response>
        /// <remarks>
        /// Priority: 0 (Urgent), 1 (High), 2 (Normal), 3 (Low)
        /// 
        /// Status: 0 (Pending), 1 (Approved), 2 (Rejected), 3 (Fulfilled)
        /// 
        /// Category: 0 (Food), 1 (Clothing), 2 (Medical), 3 (Education), 4 (Other)
        /// 
        /// Field Constraints (query):
        /// - Page: Optional, default 1, minimum 1
        /// - PageSize: Optional, default 10, max 50
        /// </remarks>
        [HttpGet("charity-needs/pending")]
        public async Task<IActionResult> GetPendingCharityNeeds([FromQuery] PendingRequestsFilterDTO query)
        {
            var result = await _adminService.GetPendingCharityNeedsAsync(query);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        /// <summary>
        /// Approves a charity need, changing its status to Approved.
        /// </summary>
        /// <param name="request">Request containing the charity need ID to approve.</param>
        /// <response code="200">Charity need approved successfully.</response>
        /// <response code="400">Validation error.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="403">Forbidden access, not an Admin.</response>
        /// <response code="404">Charity need not found.</response>
        /// <response code="422">Charity need is not in Pending status.</response>
        /// <response code="500">Unexpected server error.</response>
        /// <remarks>
        /// Field Constraints:
        /// - CharityNeedId: Required, valid GUID
        /// </remarks>
        [HttpPost("charity-needs/approve")]
        public async Task<IActionResult> ApproveCharityNeed([FromBody] ActionCharityNeedRequestDTO request)
        {
            var result = await _adminService.ApproveCharityNeedAsync(request);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        /// <summary>
        /// Rejects a charity need, changing its status to Rejected.
        /// </summary>
        /// <param name="request">Request containing the charity need ID to reject.</param>
        /// <response code="200">Charity need rejected successfully.</response>
        /// <response code="400">Validation error.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="403">Forbidden access, not an Admin.</response>
        /// <response code="404">Charity need not found.</response>
        /// <response code="422">Charity need is not in Pending status.</response>
        /// <response code="500">Unexpected server error.</response>
        /// <remarks>
        /// Field Constraints:
        /// - CharityNeedId: Required, valid GUID
        /// </remarks>
        [HttpPost("charity-needs/reject")]
        public async Task<IActionResult> RejectCharityNeed([FromBody] ActionCharityNeedRequestDTO request)
        {
            var result = await _adminService.RejectCharityNeedAsync(request);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        // =========================================================
        // OFFERS MANAGEMENT
        // =========================================================

        /// <summary>
        /// Gets pending offers awaiting admin approval.
        /// Returns offers with status Pending (0).
        /// </summary>
        /// <param name="query">Filtering and pagination parameters.</param>
        /// <response code="200">Pending offers retrieved successfully.</response>
        /// <response code="400">Invalid pagination parameters.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="403">Forbidden access, not an Admin.</response>
        /// <response code="500">Unexpected server error.</response>
        /// <remarks>
        /// Status: 0 (Pending), 1 (Approved), 2 (Rejected), 3 (Expired), 4 (Fulfilled)
        /// 
        /// Category: 0 (Food), 1 (Clothing), 2 (Medical), 3 (Education), 4 (Other)
        /// 
        /// Field Constraints (query):
        /// - Page: Optional, default 1, minimum 1
        /// - PageSize: Optional, default 10, max 50
        /// </remarks>
        [HttpGet("offers/pending")]
        public async Task<IActionResult> GetPendingOffers([FromQuery] PendingRequestsFilterDTO query)
        {
            var result = await _adminService.GetPendingOffersAsync(query);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        /// <summary>
        /// Approves an offer, changing its status to Approved.
        /// </summary>
        /// <param name="request">Request containing the offer ID to approve.</param>
        /// <response code="200">Offer approved successfully.</response>
        /// <response code="400">Validation error.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="403">Forbidden access, not an Admin.</response>
        /// <response code="404">Offer not found.</response>
        /// <response code="422">Offer is not in Pending status.</response>
        /// <response code="500">Unexpected server error.</response>
        /// <remarks>
        /// Field Constraints:
        /// - OfferId: Required, valid GUID
        /// </remarks>
        [HttpPost("offers/approve")]
        public async Task<IActionResult> ApproveOffer([FromBody] ActionOfferRequestDTO request)
        {
            var result = await _adminService.ApproveOfferAsync(request);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        /// <summary>
        /// Rejects an offer, changing its status to Rejected.
        /// </summary>
        /// <param name="request">Request containing the offer ID to reject.</param>
        /// <response code="200">Offer rejected successfully.</response>
        /// <response code="400">Validation error.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="403">Forbidden access, not an Admin.</response>
        /// <response code="404">Offer not found.</response>
        /// <response code="422">Offer is not in Pending status.</response>
        /// <response code="500">Unexpected server error.</response>
        /// <remarks>
        /// Field Constraints:
        /// - OfferId: Required, valid GUID
        /// </remarks>
        [HttpPost("offers/reject")]
        public async Task<IActionResult> RejectOffer([FromBody] ActionOfferRequestDTO request)
        {
            var result = await _adminService.RejectOfferAsync(request);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        // =========================================================
        // USER MANAGEMENT
        // =========================================================

        /// <summary>
        /// Gets all users with filters based on role and active status.
        /// </summary>
        /// <param name="query">Filtering and pagination parameters.</param>
        /// <response code="200">Users retrieved successfully.</response>
        /// <response code="400">Invalid pagination parameters.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="403">Forbidden access, not an Admin.</response>
        /// <response code="500">Unexpected server error.</response>
        /// <remarks>
        /// UserRole: 0 (Charity), 1 (DonorOrganization), 2 (Admin)
        /// 
        /// ApplicationStatus: 0 (Pending), 1 (Accepted), 2 (Rejected)
        /// 
        /// Field Constraints (query):
        /// - Page: Optional, default 1, minimum 1
        /// - PageSize: Optional, default 10, max 50
        /// - Role: Optional, valid UserRole enum (0-2)
        /// - IsActive: Optional, boolean
        /// </remarks>
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers([FromQuery] UsersFilterRequestDTO query)
        {
            var result = await _adminService.GetAllUsersAsync(query);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        /// <summary>
        /// Deactivates a user, setting IsActive to false.
        /// </summary>
        /// <param name="request">Request containing the user ID to deactivate.</param>
        /// <response code="200">User deactivated successfully.</response>
        /// <response code="400">Validation error.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="403">Forbidden access, not an Admin.</response>
        /// <response code="404">User not found.</response>
        /// <response code="500">Unexpected server error.</response>
        /// <remarks>
        /// Field Constraints:
        /// - UserId: Required, valid GUID
        /// </remarks>
        [HttpPost("users/deactivate")]
        public async Task<IActionResult> DeactivateUser([FromBody] ActionUserRequestDTO request)
        {
            var result = await _adminService.DeactivateUserAsync(request);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        /// <summary>
        /// Activates a user, setting IsActive to true.
        /// </summary>
        /// <param name="request">Request containing the user ID to activate.</param>
        /// <response code="200">User activated successfully.</response>
        /// <response code="400">Validation error.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="403">Forbidden access, not an Admin.</response>
        /// <response code="404">User not found.</response>
        /// <response code="500">Unexpected server error.</response>
        /// <remarks>
        /// Field Constraints:
        /// - UserId: Required, valid GUID
        /// </remarks>
        [HttpPost("users/activate")]
        public async Task<IActionResult> ActivateUser([FromBody] ActionUserRequestDTO request)
        {
            var result = await _adminService.ActivateUserAsync(request);
            return StatusCode((int)result.StatusCode, result.Response);
        }
    }
}
