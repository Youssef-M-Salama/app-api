using App.Core.DTOs.Request;
using App.Core.ServiceContracts;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace App.Api.Controllers.v1
{
    /// <summary>
    /// Endpoints for authenticated charity users.
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/charity")]
    //[Authorize(Roles = "Charity")]
    public class CharityController : CustomControllerBase
    {
        private readonly ICharityService _charityService;

        public CharityController(ICharityService charityService)
        {
            _charityService = charityService;
        }

        // =========================================================
        // DASHBOARD
        // =========================================================

        /// <summary>
        /// Returns aggregated statistics for the authenticated charity's dashboard.
        /// Includes charity need counts (by status), need applications received,
        /// and offer applications sent — all broken down by status.
        /// </summary>
        /// <response code="200">Dashboard statistics retrieved successfully.</response>
        /// <response code="404">Charity profile not found.</response>
        /// <response code="500">Unexpected server error.</response>
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _charityService.GetDashboardAsync(userId.Value);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        // =========================================================
        // CHARITY NEED — CRUD
        // =========================================================

        /// <summary>
        /// Creates a new charity need for the authenticated charity.
        /// The need is created with status Pending and must be approved by admin
        /// before it becomes visible to donor organizations.
        /// Accepts multipart/form-data to support an optional product image.
        /// </summary>
        /// <response code="201">Charity need created successfully.</response>
        /// <response code="400">Validation error or invalid image.</response>
        /// <response code="403">Charity account is not verified or active.</response>
        /// <response code="404">Charity profile not found.</response>
        /// <response code="500">Unexpected server error.</response>
        [HttpPost("charityNeeds")]
        public async Task<IActionResult> CreateCharityNeed(
            [FromForm] CreateCharityNeedRequestDTO request)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _charityService.CreateCharityNeedAsync(userId.Value, request);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        /// <summary>
        /// Returns a paginated list of the authenticated charity's own needs.
        /// All statuses are visible to the owner (Pending, Approved, Rejected, Fulfilled).
        /// Optionally filtered by status.
        /// </summary>
        /// <response code="200">Charity needs retrieved successfully.</response>
        /// <response code="400">Invalid pagination or status parameters.</response>
        /// <response code="404">Charity profile not found.</response>
        /// <response code="500">Unexpected server error.</response>
        [HttpGet("charityNeeds")]
        public async Task<IActionResult> GetMyCharityNeeds(
            [FromQuery] MyCharityNeedsFilterDTO query)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _charityService.GetMyCharityNeedsAsync(userId.Value, query);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        /// <summary>
        /// Returns the full detail of a single charity need owned by the authenticated charity.
        /// All statuses are visible to the owner.
        /// </summary>
        /// <param name="charityNeedId">The unique identifier of the charity need.</param>
        /// <response code="200">Charity need retrieved successfully.</response>
        /// <response code="403">The charity need does not belong to the caller.</response>
        /// <response code="404">Charity need or charity profile not found.</response>
        /// <response code="500">Unexpected server error.</response>
        [HttpGet("charityNeeds/{charityNeedId:guid}")]
        public async Task<IActionResult> GetMyCharityNeedById(Guid charityNeedId)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _charityService
                .GetMyCharityNeedByIdAsync(userId.Value, charityNeedId);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        /// <summary>
        /// Updates a pending charity need owned by the authenticated charity.
        /// Only fields provided (non-null) are applied.
        /// Accepts multipart/form-data to support an optional replacement image.
        /// </summary>
        /// <param name="charityNeedId">The unique identifier of the charity need.</param>
        /// <response code="200">Charity need updated successfully.</response>
        /// <response code="400">Validation error or invalid image.</response>
        /// <response code="403">The charity need does not belong to the caller.</response>
        /// <response code="404">Charity need or charity profile not found.</response>
        /// <response code="422">Charity need is not in Pending status.</response>
        /// <response code="500">Unexpected server error.</response>
        [HttpPut("charityNeeds/{charityNeedId:guid}")]
        public async Task<IActionResult> UpdateCharityNeed(
            Guid charityNeedId,
            [FromForm] UpdateCharityNeedRequestDTO request)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _charityService
                .UpdateCharityNeedAsync(userId.Value, charityNeedId, request);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        /// <summary>
        /// Deletes a pending charity need owned by the authenticated charity.
        /// Also removes the associated product image from storage if present.
        /// </summary>
        /// <param name="charityNeedId">The unique identifier of the charity need.</param>
        /// <response code="200">Charity need deleted successfully.</response>
        /// <response code="403">The charity need does not belong to the caller.</response>
        /// <response code="404">Charity need or charity profile not found.</response>
        /// <response code="422">Charity need is not in Pending status.</response>
        /// <response code="500">Unexpected server error.</response>
        [HttpDelete("charityNeeds/{charityNeedId:guid}")]
        public async Task<IActionResult> DeleteCharityNeed(Guid charityNeedId)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _charityService
                .DeleteCharityNeedAsync(userId.Value, charityNeedId);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        /// <summary>
        /// Marks an approved charity need as fulfilled.
        /// The need must be in Approved status — only approved needs can be fulfilled.
        /// </summary>
        /// <param name="charityNeedId">The unique identifier of the charity need.</param>
        /// <response code="200">Charity need marked as fulfilled.</response>
        /// <response code="403">The charity need does not belong to the caller.</response>
        /// <response code="404">Charity need or charity profile not found.</response>
        /// <response code="422">Charity need is not in Approved status.</response>
        /// <response code="500">Unexpected server error.</response>
        [HttpPost("charityNeeds/{charityNeedId:guid}/fulfill")]
        public async Task<IActionResult> FulfillCharityNeed(Guid charityNeedId)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _charityService
                .FulfillCharityNeedAsync(userId.Value, charityNeedId);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        // =========================================================
        // NEED APPLICATIONS — received (donors applied to my needs)
        // =========================================================

        /// <summary>
        /// Returns a paginated list of need applications received by the charity
        /// across all of its charity needs.
        /// </summary>
        /// <response code="200">Applications retrieved successfully.</response>
        /// <response code="400">Invalid pagination parameters.</response>
        /// <response code="404">Charity profile not found.</response>
        /// <response code="500">Unexpected server error.</response>
        [HttpGet("applications/received")]
        public async Task<IActionResult> GetReceivedApplications(
            [FromQuery] PaginationFilterDTO query)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _charityService
                .GetReceivedApplicationsAsync(userId.Value, query);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        /// <summary>
        /// Accepts a pending need application received by the charity.
        /// </summary>
        /// <param name="needApplicationId">The unique identifier of the need application.</param>
        /// <response code="200">Need application accepted.</response>
        /// <response code="403">The application does not belong to this charity's need.</response>
        /// <response code="404">Application or charity profile not found.</response>
        /// <response code="422">Application is not in Pending status.</response>
        /// <response code="500">Unexpected server error.</response>
        [HttpPost("applications/{needApplicationId:guid}/accept")]
        public async Task<IActionResult> AcceptNeedApplication(Guid needApplicationId)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _charityService
                .AcceptNeedApplicationAsync(userId.Value, needApplicationId);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        /// <summary>
        /// Rejects a pending need application received by the charity.
        /// </summary>
        /// <param name="needApplicationId">The unique identifier of the need application.</param>
        /// <response code="200">Need application rejected.</response>
        /// <response code="403">The application does not belong to this charity's need.</response>
        /// <response code="404">Application or charity profile not found.</response>
        /// <response code="422">Application is not in Pending status.</response>
        /// <response code="500">Unexpected server error.</response>
        [HttpPost("applications/{needApplicationId:guid}/reject")]
        public async Task<IActionResult> RejectNeedApplication(Guid needApplicationId)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _charityService
                .RejectNeedApplicationAsync(userId.Value, needApplicationId);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        // =========================================================
        // OFFER APPLICATIONS — sent (I applied to donor offers)
        // =========================================================

        /// <summary>
        /// Applies the authenticated charity to an approved donor offer.
        /// A charity can only apply once per offer.
        /// </summary>
        /// <param name="offerId">The unique identifier of the offer.</param>
        /// <response code="201">Application submitted successfully.</response>
        /// <response code="403">Charity account is not verified or active.</response>
        /// <response code="404">Offer not found or no longer available.</response>
        /// <response code="409">Already applied to this offer.</response>
        /// <response code="500">Unexpected server error.</response>
        [HttpPost("offers/{offerId:guid}/apply")]
        public async Task<IActionResult> ApplyToOffer(Guid offerId)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _charityService.ApplyToOfferAsync(userId.Value, offerId);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        /// <summary>
        /// Returns a paginated list of offer applications sent by the charity.
        /// </summary>
        /// <response code="200">Applications retrieved successfully.</response>
        /// <response code="400">Invalid pagination parameters.</response>
        /// <response code="404">Charity profile not found.</response>
        /// <response code="500">Unexpected server error.</response>
        [HttpGet("applications/sent")]
        public async Task<IActionResult> GetSentApplications(
            [FromQuery] PaginationFilterDTO query)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _charityService
                .GetSentApplicationsAsync(userId.Value, query);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        /// <summary>
        /// Cancels a pending offer application sent by the charity.
        /// </summary>
        /// <param name="offerApplicationId">The unique identifier of the offer application.</param>
        /// <response code="200">Offer application cancelled.</response>
        /// <response code="403">The application does not belong to the caller.</response>
        /// <response code="404">Application or charity profile not found.</response>
        /// <response code="422">Application is not in Pending status.</response>
        /// <response code="500">Unexpected server error.</response>
        [HttpDelete("applications/{offerApplicationId:guid}")]
        public async Task<IActionResult> CancelOfferApplication(Guid offerApplicationId)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _charityService
                .CancelOfferApplicationAsync(userId.Value, offerApplicationId);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        // =========================================================
        // PRIVATE HELPERS
        // =========================================================

        private Guid? GetUserId()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(claim) || !Guid.TryParse(claim, out var userId))
                return null;
            return userId;
        }
    }
}