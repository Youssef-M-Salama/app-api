using App.Core.DTOs.Request;
using App.Core.ServiceContracts;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace App.Api.Controllers.v1
{
    /// <summary>
    /// Endpoints for authenticated donor organization users.
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/donor-organization")]
    [Authorize(Roles = "DonorOrganization")]
    [Tags("Donor Organization")]
    public class DonorOrganizationController : CustomControllerBase
    {
        private readonly IDonorOrganizationService _donorOrganizationService;

        public DonorOrganizationController(IDonorOrganizationService donorOrganizationService)
        {
            _donorOrganizationService = donorOrganizationService;
        }

        // =========================================================
        // DASHBOARD
        // =========================================================

        /// <summary>
        /// Returns aggregated statistics for the authenticated donor organization's dashboard.
        /// Includes offer counts (by status), applications received, and applications sent — all broken down by status.
        /// </summary>
        /// <response code="200">Dashboard statistics retrieved successfully.</response>
        /// <response code="404">Donor organization profile not found.</response>
        /// <response code="500">Unexpected server error.</response>
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var userId = GetUserId();
            if (userId is null)
            {
                return Unauthorized();
            }
            var result = await _donorOrganizationService.GetDashboardAsync(userId.Value);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        // =========================================================
        // OFFER — CRUD
        // =========================================================

        /// <summary>
        /// Creates a new offer for the authenticated donor organization.
        /// The offer is created with status Pending and must be approved by admin
        /// before it becomes visible to charities.
        /// Accepts multipart/form-data to support an optional product image.
        /// </summary>
        /// <param name="request">Offer details including product name, quantity, category, expiry date, and optional image.</param>
        /// <response code="201">Offer created successfully.</response>
        /// <response code="400">Validation error or invalid image.</response>
        /// <response code="403">Donor organization account is not verified or active.</response>
        /// <response code="404">Donor organization profile not found.</response>
        /// <response code="500">Unexpected server error.</response>
        /// <remarks>
        /// Category: 0 (Food), 1 (Clothing), 2 (Medical), 3 (Education), 4 (Other)
        /// 
        /// Field Constraints:
        /// - ProductName: Required, max 200 characters
        /// - Quantity: Required, minimum 1
        /// - Category: Required, valid ProductCategory enum
        /// - ExpiryDate: Required, must be a future date
        /// - ProductImage: Optional, allowed formats: .jpg, .jpeg, .png, .webp, max 2MB
        /// </remarks>
        [HttpPost("offer")]
        public async Task<IActionResult> CreateOffer([FromForm] CreateOfferRequestDTO request)
        {
            var userId = GetUserId();
            if (userId is null)
            {
                return Unauthorized();
            }
            var result = await _donorOrganizationService.CreateOfferAsync(userId.Value, request);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        /// <summary>
        /// Returns a paginated list of the authenticated donor organization's own offers.
        /// All statuses are visible to the owner (Pending, Approved, Rejected, Fulfilled, Expired).
        /// Optionally filtered by status.
        /// </summary>
        /// <param name="query">Filtering and pagination parameters.</param>
        /// <response code="200">Offers retrieved successfully.</response>
        /// <response code="400">Invalid pagination or status parameters.</response>
        /// <response code="404">Donor organization profile not found.</response>
        /// <response code="500">Unexpected server error.</response>
        /// <remarks>
        /// Category: 0 (Food), 1 (Clothing), 2 (Medical), 3 (Education), 4 (Other)
        /// 
        /// Status: 0 (Pending), 1 (Approved), 2 (Rejected), 3 (Expired), 4 (Fulfilled)
        /// </remarks>
        [HttpGet("offer/my-offers")]
        public async Task<IActionResult> GetMyOffers([FromQuery] MyOffersFilterDTO query)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _donorOrganizationService.GetMyOffersAsync(userId.Value, query);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        /// <summary>
        /// Returns the full detail of a single offer owned by the authenticated donor organization.
        /// All statuses are visible to the owner.
        /// </summary>
        /// <param name="offerId">The unique identifier of the offer.</param>
        /// <response code="200">Offer retrieved successfully.</response>
        /// <response code="403">The offer does not belong to the caller.</response>
        /// <response code="404">Offer or donor organization profile not found.</response>
        /// <response code="500">Unexpected server error.</response>
        /// <remarks>
        /// Category: 0 (Food), 1 (Clothing), 2 (Medical), 3 (Education), 4 (Other)
        /// 
        /// Status: 0 (Pending), 1 (Approved), 2 (Rejected), 3 (Expired), 4 (Fulfilled)
        /// </remarks>
        [HttpGet("offer/my-offers/{offerId}")]
        public async Task<IActionResult> GetMyOfferById(Guid offerId)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _donorOrganizationService.GetMyOfferByIdAsync(userId.Value, offerId);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        /// <summary>
        /// Updates a pending offer owned by the authenticated donor organization.
        /// Only fields provided (non-null) are applied.
        /// Accepts multipart/form-data to support an optional replacement image.
        /// </summary>
        /// <param name="offerId">The unique identifier of the offer.</param>
        /// <param name="request">Updated offer details.</param>
        /// <response code="200">Offer updated successfully.</response>
        /// <response code="400">Validation error or invalid image.</response>
        /// <response code="403">The offer does not belong to the caller.</response>
        /// <response code="404">Offer or donor organization profile not found.</response>
        /// <response code="422">Offer is not in Pending status.</response>
        /// <response code="500">Unexpected server error.</response>
        /// <remarks>
        /// Field Constraints:
        /// - ProductName: Optional, max 200 characters
        /// - Quantity: Optional, minimum 1
        /// - Category: Optional, valid ProductCategory enum
        /// - ExpiryDate: Optional, must be a future date
        /// - ProductImage: Optional, allowed formats: .jpg, .jpeg, .png, .webp, max 2MB
        /// </remarks>
        [HttpPut("offer/{offerId}")]
        public async Task<IActionResult> UpdateOffer(Guid offerId, [FromForm] UpdateOfferRequestDTO request)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _donorOrganizationService.UpdateOfferAsync(userId.Value, offerId, request);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        /// <summary>
        /// Deletes a pending offer owned by the authenticated donor organization.
        /// Also removes the associated product image from storage if present.
        /// </summary>
        /// <param name="offerId">The unique identifier of the offer.</param>
        /// <response code="200">Offer deleted successfully.</response>
        /// <response code="403">The offer does not belong to the caller.</response>
        /// <response code="404">Offer or donor organization profile not found.</response>
        /// <response code="422">Offer is not in Pending status.</response>
        /// <response code="500">Unexpected server error.</response>
        [HttpDelete("offer/{offerId}")]
        public async Task<IActionResult> DeleteOffer(Guid offerId)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _donorOrganizationService.DeleteOfferAsync(userId.Value, offerId);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        /// <summary>
        /// Marks an approved offer as fulfilled.
        /// The offer must be in Approved status — only approved offers can be fulfilled.
        /// </summary>
        /// <param name="offerId">The unique identifier of the offer.</param>
        /// <response code="200">Offer marked as fulfilled.</response>
        /// <response code="403">The offer does not belong to the caller.</response>
        /// <response code="404">Offer or donor organization profile not found.</response>
        /// <response code="422">Offer is not in Approved status.</response>
        /// <response code="500">Unexpected server error.</response>
        [HttpPatch("offer/{offerId}/fulfill")]
        public async Task<IActionResult> FulfillOffer(Guid offerId)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _donorOrganizationService.FulfillOfferAsync(userId.Value, offerId);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        // =========================================================
        // OFFER APPLICATIONS — received (charities applied to my offers)
        // =========================================================

        /// <summary>
        /// Returns a paginated list of offer applications received by the donor organization
        /// across all of its offers.
        /// </summary>
        /// <param name="query">Pagination parameters.</param>
        /// <response code="200">Applications retrieved successfully.</response>
        /// <response code="400">Invalid pagination parameters.</response>
        /// <response code="404">Donor organization profile not found.</response>
        /// <response code="500">Unexpected server error.</response>
        /// <remarks>
        /// Category: 0 (Food), 1 (Clothing), 2 (Medical), 3 (Education), 4 (Other)
        /// 
        /// Status: 0 (Pending), 1 (Approved), 2 (Rejected), 3 (Expired), 4 (Fulfilled)
        /// </remarks>
        [HttpGet("offer-applications/received")]
        public async Task<IActionResult> GetReceivedApplications([FromQuery] PaginationFilterDTO query)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _donorOrganizationService.GetReceivedApplicationsAsync(userId.Value, query);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        /// <summary>
        /// Accepts a pending offer application received by the donor organization.
        /// </summary>
        /// <param name="offerApplicationId">The unique identifier of the offer application.</param>
        /// <response code="200">Offer application accepted.</response>
        /// <response code="403">The application does not belong to this donor organization's offer.</response>
        /// <response code="404">Application or donor organization profile not found.</response>
        /// <response code="422">Application is not in Pending status.</response>
        /// <response code="500">Unexpected server error.</response>
        [HttpPatch("offer-applications/{offerApplicationId}/accept")]
        public async Task<IActionResult> AcceptOfferApplication(Guid offerApplicationId)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _donorOrganizationService.AcceptOfferApplicationAsync(userId.Value, offerApplicationId);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        /// <summary>
        /// Rejects a pending offer application received by the donor organization.
        /// </summary>
        /// <param name="offerApplicationId">The unique identifier of the offer application.</param>
        /// <response code="200">Offer application rejected.</response>
        /// <response code="403">The application does not belong to this donor organization's offer.</response>
        /// <response code="404">Application or donor organization profile not found.</response>
        /// <response code="422">Application is not in Pending status.</response>
        /// <response code="500">Unexpected server error.</response>
        [HttpPatch("offer-applications/{offerApplicationId}/reject")]
        public async Task<IActionResult> RejectOfferApplication(Guid offerApplicationId)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _donorOrganizationService.RejectOfferApplicationAsync(userId.Value, offerApplicationId);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        // =========================================================
        // CHARITY NEED APPLICATIONS — sent (I applied to charity needs)
        // =========================================================

        /// <summary>
        /// Applies the authenticated donor organization to an approved charity need.
        /// A donor organization can only apply once per need.
        /// </summary>
        /// <param name="charityNeedId">The unique identifier of the charity need.</param>
        /// <response code="201">Application submitted successfully.</response>
        /// <response code="403">Donor organization account is not verified or active.</response>
        /// <response code="404">Charity need not found or no longer available.</response>
        /// <response code="409">Already applied to this charity need.</response>
        /// <response code="500">Unexpected server error.</response>
        [HttpPost("charity-needs/{charityNeedId}/apply")]
        public async Task<IActionResult> ApplyToCharityNeed(Guid charityNeedId)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _donorOrganizationService.ApplyToCharityNeedAsync(userId.Value, charityNeedId);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        /// <summary>
        /// Returns a paginated list of need applications sent by the donor organization
        /// to various charity needs.
        /// </summary>
        /// <param name="query">Pagination parameters.</param>
        /// <response code="200">Applications retrieved successfully.</response>
        /// <response code="400">Invalid pagination parameters.</response>
        /// <response code="404">Donor organization profile not found.</response>
        /// <response code="500">Unexpected server error.</response>
        [HttpGet("need-applications/sent")]
        public async Task<IActionResult> GetSentApplications([FromQuery] PaginationFilterDTO query)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _donorOrganizationService.GetSentApplicationsAsync(userId.Value, query);
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
