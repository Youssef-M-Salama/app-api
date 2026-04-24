using App.Core.DTOs.Request;
using App.Core.DTOs.Response;
using App.Core.DTOs.ResultPattern;
using App.Core.ServiceContracts;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers.V1
{
    /// <summary>
    /// Public endpoints accessible without authentication.
    /// Provides read-only access to approved charity needs and offers.
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/public")]
    public class PublicController : CustomControllerBase
    {
        private readonly IPublicService _publicService;

        public PublicController(IPublicService publicService)
        {
            _publicService = publicService;
        }

        // =========================================================
        // CHARITY NEEDS
        // =========================================================

        /// <summary>
        /// Browse all approved charity requests with optional filters and pagination.
        /// Only returns charity needs with status Approved (1).
        /// </summary>
        /// <param name="query">Filtering and pagination parameters.</param>
        /// <response code="200">Requests retrieved successfully.</response>
        /// <response code="400">Invalid pagination parameters.</response>
        /// <response code="500">Unexpected server error.</response>
        /// <remarks>
        /// Category: 0 (Food), 1 (Clothing), 2 (Medical), 3 (Education), 4 (Other)
        /// 
        /// Priority: 0 (Urgent), 1 (High), 2 (Normal), 3 (Low)
        /// 
        /// Status: 0 (Pending), 1 (Approved), 2 (Rejected), 3 (Fulfilled)
        /// 
        /// Field Constraints (query):
        /// - Page: Optional, default 1, minimum 1
        /// - PageSize: Optional, default 10, max 50
        /// - Category: Optional, valid ProductCategory enum (0-4)
        /// - Priority: Optional, valid CharityNeedPriority enum (0-3)
        /// - City: Optional, filter by city
        /// - Governorate: Optional, filter by governorate
        /// - Search: Optional, search by product name
        /// </remarks>
        [HttpGet("charity-needs")]
        public async Task<IActionResult> GetApprovedCharityNeeds([FromQuery] ApprovedCharityNeedsRequestDTO query)
        {
            var result = await _publicService.GetApprovedCharityNeedsAsync(query);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        /// <summary>
        /// Returns a single approved charity need by its identifier.
        /// Only returns charity needs with status Approved (1).
        /// </summary>
        /// <param name="charityNeedId">The unique identifier of the charity need.</param>
        /// <response code="200">CharityNeed retrieved successfully.</response>
        /// <response code="404">CharityNeed not found or not approved.</response>
        /// <response code="500">Unexpected server error.</response>
        /// <remarks>
        /// Category: 0 (Food), 1 (Clothing), 2 (Medical), 3 (Education), 4 (Other)
        /// 
        /// Priority: 0 (Urgent), 1 (High), 2 (Normal), 3 (Low)
        /// 
        /// Status: 0 (Pending), 1 (Approved), 2 (Rejected), 3 (Fulfilled)
        /// </remarks>
        [HttpGet("charity-needs/{charityNeedId:guid}")]
        public async Task<IActionResult> GetApprovedCharityNeedById(Guid charityNeedId)
        {
            var result = await _publicService.GetApprovedCharityNeedByIdAsync(charityNeedId);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        // =========================================================
        // OFFERS
        // =========================================================

        /// <summary>
        /// Browse all approved offers with optional filters and pagination.
        /// Only returns offers with status Approved (1).
        /// </summary>
        /// <param name="query">Filtering and pagination parameters.</param>
        /// <response code="200">Offers retrieved successfully.</response>
        /// <response code="400">Invalid pagination parameters.</response>
        /// <response code="500">Unexpected server error.</response>
        /// <remarks>
        /// Category: 0 (Food), 1 (Clothing), 2 (Medical), 3 (Education), 4 (Other)
        /// 
        /// Status: 0 (Pending), 1 (Approved), 2 (Rejected), 3 (Fulfilled), 4 (Expired)
        /// 
        /// Field Constraints (query):
        /// - Page: Optional, default 1, minimum 1
        /// - PageSize: Optional, default 10, max 50
        /// - Category: Optional, valid ProductCategory enum (0-4)
        /// - City: Optional, filter by city
        /// - Governorate: Optional, filter by governorate
        /// - Search: Optional, search by product name
        /// </remarks>
        [HttpGet("offers")]
        public async Task<IActionResult> GetApprovedOffers([FromQuery] ApprovedOffersRequestDTO query)
        {
            var result = await _publicService.GetApprovedOffersAsync(query);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        /// <summary>
        /// Returns a single approved offer by its identifier.
        /// Only returns offers with status Approved (1).
        /// </summary>
        /// <param name="offerId">The unique identifier of the offer.</param>
        /// <response code="200">Offer retrieved successfully.</response>
        /// <response code="404">Offer not found or not approved.</response>
        /// <response code="500">Unexpected server error.</response>
        /// <remarks>
        /// Category: 0 (Food), 1 (Clothing), 2 (Medical), 3 (Education), 4 (Other)
        /// 
        /// Status: 0 (Pending), 1 (Approved), 2 (Rejected), 3 (Fulfilled), 4 (Expired)
        /// </remarks>
        [HttpGet("offers/{offerId:guid}")]
        public async Task<IActionResult> GetApprovedOfferById(Guid offerId)
        {
            var result = await _publicService.GetApprovedOfferByIdAsync(offerId);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        // =========================================================
        // STATISTICS
        // =========================================================

        /// <summary>
        /// Returns platform-wide statistics for the public landing page.
        /// Includes total approved charity needs, total approved offers, and user counts.
        /// </summary>
        /// <response code="200">Statistics retrieved successfully.</response>
        /// <response code="500">Unexpected server error.</response>
        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics()
        {
            var result = await _publicService.GetStatisticsAsync();
            return StatusCode((int)result.StatusCode, result.Response);
        }
    }
}