using App.Core.DTO.Request;
using App.Core.DTO.Response;
using App.Core.DTO.ResultPattern;
using App.Core.ServiceContracts;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers.V1
{
    /// <summary>
    /// Public endpoints accessible without authentication.
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

        /// <summary>
        /// Browse all approved charity requests with optional filters and pagination.
        /// </summary>
        /// <param name="query">Filtering and pagination parameters.</param>
        /// <response code="200">Requests retrieved successfully.</response>
        /// <response code="400">Invalid pagination parameters.</response>
        /// <response code="500">Unexpected server error.</response>
        [HttpGet("charityNeeds")]
        public async Task<IActionResult> GetApprovedCharityNeeds([FromQuery] ApprovedCharityNeedsRequestDto query)
        {
            var result = await _publicService.GetApprovedCharityNeedsAsync(query);
            return StatusCode((int)result.StatusCode, result.Response);
        }
        /// <summary>
        /// Returns platform-wide statistics for the public landing page.
        /// </summary>
        /// <response code="200">Statistics retrieved successfully.</response>
        /// <response code="500">Unexpected server error.</response>
        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics()
        {
            var result = await _publicService.GetStatisticsAsync();
            return StatusCode((int)result.StatusCode, result.Response);
        }
        /// <summary>
        /// Browse all approved offers with optional filters and pagination.
        /// </summary>
        /// <param name="query">Filtering and pagination parameters.</param>
        /// <response code="200">Offers retrieved successfully.</response>
        /// <response code="400">Invalid pagination parameters.</response>
        /// <response code="500">Unexpected server error.</response>
        [HttpGet("offers")]
        public async Task<IActionResult> GetApprovedOffers([FromQuery] ApprovedOffersRequestDto query)
        {
            var result = await _publicService.GetApprovedOffersAsync(query);
            return StatusCode((int)result.StatusCode, result.Response);
        }
        /// <summary>
        /// Returns a single approved charity need by its identifier.
        /// </summary>
        /// <param name="charityNeedId">The unique identifier of the charity need.</param>
        /// <response code="200">CharityNeed retrieved successfully.</response>
        /// <response code="404">CharityNeed not found or not approved.</response>
        /// <response code="500">Unexpected server error.</response>
        [HttpGet("charityNeeds/{charityNeedId:guid}")]
        public async Task<IActionResult> GetApprovedCharityNeedById(Guid charityNeedId)
        {
            var result = await _publicService.GetApprovedCharityNeedByIdAsync(charityNeedId);
            return StatusCode((int)result.StatusCode, result.Response);
        }
        /// <summary>
        /// Returns a single approved offer by its identifier.
        /// </summary>
        /// <param name="offerId">The unique identifier of the offer.</param>
        /// <response code="200">Offer retrieved successfully.</response>
        /// <response code="404">Offer not found or not approved.</response>
        /// <response code="500">Unexpected server error.</response>
        [HttpGet("offers/{offerId:guid}")]
        public async Task<IActionResult> GetApprovedOfferById(Guid offerId)
        {
            var result = await _publicService.GetApprovedOfferByIdAsync(offerId);
            return StatusCode((int)result.StatusCode, result.Response);
        }
    }
}