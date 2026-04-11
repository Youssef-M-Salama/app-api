using App.Core.ServiceContracts;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/admin")]
    //[Authorize(Roles = "Admin")]
    public class AdminController : CustomControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        /// <summary>
        /// Retrieves aggregated dashboard statistics for Admin.
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

        /// <summary>
        /// Retrieves lists of unverified charities and donor organizations.
        /// </summary>
        /// <response code="200">Pending verifications retrieved successfully.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="403">Forbidden access, not an Admin.</response>
        /// <response code="500">Unexpected server error.</response>
        [HttpGet("verifications/pending")]
        public async Task<IActionResult> GetPendingVerifications()
        {
            var result = await _adminService.GetPendingVerificationsAsync();
            return StatusCode((int)result.StatusCode, result.Response);
        }

        /// <summary>
        /// Verifies a user.
        /// </summary>
        [HttpPost("verifications/verify")]
        public async Task<IActionResult> VerifyUser([FromBody] App.Core.DTOs.Request.ActionUserRequestDTO request)
        {
            var result = await _adminService.VerifyUserAsync(request);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        /// <summary>
        /// Rejects a user registration.
        /// </summary>
        [HttpPost("verifications/reject")]
        public async Task<IActionResult> RejectUser([FromBody] App.Core.DTOs.Request.ActionUserRequestDTO request)
        {
            var result = await _adminService.RejectUserAsync(request);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        /// <summary>
        /// Gets pending charity needs.
        /// <remarks>
        ///         /// Returns Priority: 0 (Urgent), 1 (High), 2 (Normal), 3 (Low).
        ///         /// Returns Status: 0 (Pending), 1 (Approved), 2 (Rejected), 3 (Fulfilled).
        ///         /// </remarks>
        /// </summary>
        /// <remarks>

        ///         /// Returns Priority: 0 (Urgent), 1 (High), 2 (Normal), 3 (Low).

        ///         /// Returns Status: 0 (Pending), 1 (Approved), 2 (Rejected), 3 (Fulfilled).

        ///         /// </remarks>
        [HttpGet("charityNeeds/pending")]
        public async Task<IActionResult> GetPendingCharityNeeds([FromQuery] App.Core.DTOs.Request.PendingRequestsFilterDTO query)
        {
            var result = await _adminService.GetPendingCharityNeedsAsync(query);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        /// <summary>
        /// Approves a charity need.
        /// </summary>
        [HttpPost("charityNeeds/approve")]
        public async Task<IActionResult> ApproveCharityNeed([FromBody] App.Core.DTOs.Request.ActionCharityNeedRequestDTO request)
        {
            var result = await _adminService.ApproveCharityNeedAsync(request);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        /// <summary>
        /// Rejects a charity need.
        /// </summary>
        [HttpPost("charityNeeds/reject")]
        public async Task<IActionResult> RejectCharityNeed([FromBody] App.Core.DTOs.Request.ActionCharityNeedRequestDTO request)
        {
            var result = await _adminService.RejectCharityNeedAsync(request);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        /// <summary>
        /// Gets pending offers awaiting admin approval.
        /// <remarks>
        ///         /// Returns Status: 0 (Pending), 1 (Approved), 2 (Rejected), 3 (Expired), 4 (Fulfilled).
        ///         /// </remarks>
        /// </summary>
        /// <remarks>

        ///         /// Returns Status: 0 (Pending), 1 (Approved), 2 (Rejected), 3 (Expired), 4 (Fulfilled).

        ///         /// </remarks>
        [HttpGet("offers/pending")]
        public async Task<IActionResult> GetPendingOffers([FromQuery] App.Core.DTOs.Request.PendingRequestsFilterDTO query)
        {
            var result = await _adminService.GetPendingOffersAsync(query);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        /// <summary>
        /// Approves an offer.
        /// </summary>
        [HttpPost("offers/approve")]
        public async Task<IActionResult> ApproveOffer([FromBody] App.Core.DTOs.Request.ActionOfferRequestDTO request)
        {
            var result = await _adminService.ApproveOfferAsync(request);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        /// <summary>
        /// Rejects an offer.
        /// </summary>
        [HttpPost("offers/reject")]
        public async Task<IActionResult> RejectOffer([FromBody] App.Core.DTOs.Request.ActionOfferRequestDTO request)
        {
            var result = await _adminService.RejectOfferAsync(request);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        /// <summary>
        /// Gets all users with filters based on role and active status.
        /// </summary>
        /// <remarks>
        /// Role values: 0 (Admin), 1 (Charity), 2 (DonorOrganization).
        /// </remarks>
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers([FromQuery] App.Core.DTOs.Request.UsersFilterRequestDTO query)
        {
            var result = await _adminService.GetAllUsersAsync(query);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        /// <summary>
        /// Deactivates a user.
        /// </summary>
        [HttpPost("users/deactivate")]
        public async Task<IActionResult> DeactivateUser([FromBody] App.Core.DTOs.Request.ActionUserRequestDTO request)
        {
            var result = await _adminService.DeactivateUserAsync(request);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        /// <summary>
        /// Activates a user.
        /// </summary>
        [HttpPost("users/activate")]
        public async Task<IActionResult> ActivateUser([FromBody] App.Core.DTOs.Request.ActionUserRequestDTO request)
        {
            var result = await _adminService.ActivateUserAsync(request);
            return StatusCode((int)result.StatusCode, result.Response);
        }
    }
}
