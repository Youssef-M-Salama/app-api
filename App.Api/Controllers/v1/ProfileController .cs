using App.Core.DTOs.Request;
using App.Core.ServiceContracts;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace App.Api.Controllers.v1
{
    [ApiVersion("1.0")]
    //[Authorize]
    public class ProfileController : CustomControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        /// <summary>
        /// Returns the full profile of the authenticated user.
        /// </summary>
        /// <response code="200">Profile retrieved successfully.</response>
        /// <response code="404">User not found.</response>
        /// <response code="500">Unexpected server error.</response>
        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var result = await _profileService.GetProfileAsync(userId.Value);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        /// <summary>
        /// Updates the authenticated user's contact and location fields.
        /// </summary>
        /// <response code="200">Profile updated successfully.</response>
        /// <response code="400">Validation error.</response>
        /// <response code="404">User not found.</response>
        /// <response code="500">Unexpected server error.</response>
        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequestDTO request)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var result = await _profileService.UpdateProfileAsync(userId.Value, request);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        /// <summary>
        /// Changes the authenticated user's password.
        /// </summary>
        /// <response code="200">Password changed successfully.</response>
        /// <response code="400">Validation error.</response>
        /// <response code="404">User not found.</response>
        /// <response code="500">Unexpected server error.</response>
        [HttpPatch("password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDTO request)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var result = await _profileService.ChangePasswordAsync(userId.Value, request);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        /// <summary>
        /// Updates the authenticated user's profile image.
        /// </summary>
        /// <response code="200">Image updated successfully.</response>
        /// <response code="400">Invalid image.</response>
        /// <response code="404">User not found.</response>
        /// <response code="500">Unexpected server error.</response>
        [HttpPatch("image")]
        public async Task<IActionResult> UpdateProfileImage([FromForm] UpdateProfileImageRequestDTO request)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var result = await _profileService.UpdateProfileImageAsync(userId.Value, request);
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