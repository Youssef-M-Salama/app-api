using App.Core.DTOs.Request;
using App.Core.ServiceContracts;
using App.Core.Services;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace App.Api.Controllers.v1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/DonorOrganization")]
    public class DonorOrganizationController : CustomControllerBase
    {
        private readonly  IDonorOrganizationService _donorOrganizationService;

        public DonorOrganizationController(IDonorOrganizationService donorOrganizationService)
        {
            _donorOrganizationService = donorOrganizationService;
        }
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
        
        [HttpGet("offer/my-offers")]
        public async Task<IActionResult> GetMyOffers([FromQuery] MyOffersFilterDTO query)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _donorOrganizationService.GetMyOffersAsync(userId.Value, query);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        [HttpGet("offer/my-offers/{offerId}")]
        public async Task<IActionResult> GetMyOfferById(Guid offerId)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _donorOrganizationService.GetMyOfferByIdAsync(userId.Value, offerId);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        [HttpPut("offer/{offerId}")]
        public async Task<IActionResult> UpdateOffer(Guid offerId, [FromForm] UpdateOfferRequestDTO request)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _donorOrganizationService.UpdateOfferAsync(userId.Value, offerId, request);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        [HttpDelete("offer/{offerId}")]
        public async Task<IActionResult> DeleteOffer(Guid offerId)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _donorOrganizationService.DeleteOfferAsync(userId.Value, offerId);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        [HttpPatch("offer/{offerId}/fulfill")]
        public async Task<IActionResult> FulfillOffer(Guid offerId)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _donorOrganizationService.FulfillOfferAsync(userId.Value, offerId);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        [HttpGet("offer-applications/received")]
        public async Task<IActionResult> GetReceivedApplications([FromQuery] PaginationFilterDTO query)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _donorOrganizationService.GetReceivedApplicationsAsync(userId.Value, query);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        [HttpPatch("offer-applications/{offerApplicationId}/accept")]
        public async Task<IActionResult> AcceptOfferApplication(Guid offerApplicationId)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _donorOrganizationService.AcceptOfferApplicationAsync(userId.Value, offerApplicationId);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        [HttpPatch("offer-applications/{offerApplicationId}/reject")]
        public async Task<IActionResult> RejectOfferApplication(Guid offerApplicationId)
        {
            var userId = GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _donorOrganizationService.RejectOfferApplicationAsync(userId.Value, offerApplicationId);
            return StatusCode((int)result.StatusCode, result.Response);
        }
private Guid? GetUserId()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(claim) || !Guid.TryParse(claim, out var userId))
                return null;
            return userId;
        }
    }
}
