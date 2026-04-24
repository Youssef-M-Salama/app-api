using App.Core.DTOs.Request;
using App.Core.ServiceContracts;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace App.Api.Controllers.v1
{
    /// <summary>
    /// Profile endpoints for authenticated users to manage their profile information.
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/profile")]
    [Authorize]
    public class ProfileController : CustomControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        /// <summary>
        /// Returns the full profile of the authenticated user.
        /// Includes contact information, location details, and profile image URL.
        /// </summary>
        /// <response code="200">Profile retrieved successfully.</response>
        /// <response code="401">Unauthorized access.</response>
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
        /// All fields are optional — only non-null values are applied.
        /// </summary>
        /// <param name="request">Profile update details including phone, whatsapp, city, governorate, and postal code.</param>
        /// <response code="200">Profile updated successfully.</response>
        /// <response code="400">Validation error.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="404">User not found.</response>
        /// <response code="500">Unexpected server error.</response>
        /// <remarks>
        /// Field Constraints:
        /// - Phone: Optional, valid phone number format
        /// - Whatsapp: Optional, valid phone number format
        /// - City: Optional, max 100 characters
        /// - Governorate: Optional, max 100 characters
        /// - PostalCode: Optional, max 20 characters
        /// </remarks>
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
        /// Requires the current password to be verified before setting a new password.
        /// </summary>
        /// <param name="request">Password change details including current password, new password, and confirmation.</param>
        /// <response code="200">Password changed successfully.</response>
        /// <response code="400">Validation error or current password is incorrect.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="404">User not found.</response>
        /// <response code="500">Unexpected server error.</response>
        /// <remarks>
        /// Field Constraints:
        /// - CurrentPassword: Required, must match the current password
        /// - NewPassword: Required, minimum 5 characters
        /// - ConfirmPassword: Required, must match NewPassword
        /// </remarks>
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
        /// Accepts multipart/form-data with the image file.
        /// </summary>
        /// <param name="request">Profile image update request containing the image file.</param>
        /// <response code="200">Image updated successfully.</response>
        /// <response code="400">Invalid image format or size.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="404">User not found.</response>
        /// <response code="500">Unexpected server error.</response>
        /// <remarks>
        /// Field Constraints:
        /// - ProfileImage: Required, allowed formats: .jpg, .jpeg, .png, .webp, max 2MB
        /// </remarks>
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