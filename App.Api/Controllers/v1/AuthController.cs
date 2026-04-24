using App.Core.DTOs.Request;
using App.Core.ServiceContracts;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace App.Api.Controllers.v1
{
    /// <summary>
    /// Authentication endpoints for user registration, login, and token management.
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/auth")]
    public class AuthController : CustomControllerBase
    {
        private readonly IAccountService _accountService;

        public AuthController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        /// <summary>
        /// Register a new charity or donor organization account.
        /// Returns 201 Created with a message to check email for verification.
        /// </summary>
        /// <param name="request">Registration details including account type, name, username, email, phone, password, and optional profile information.</param>
        /// <response code="201">Account created successfully. Verification email sent.</response>
        /// <response code="400">Validation error - invalid input or account type.</response>
        /// <response code="409">Username or email already exists.</response>
        /// <response code="500">Unexpected server error.</response>
        /// <remarks>
        /// AccountType: 0 (Charity), 1 (DonorOrganization)
        /// 
        /// Field Constraints:
        /// - AccountType: Required, valid AccountType enum
        /// - Name: Required, 3-200 characters
        /// - Username: Required, 3-50 characters, must be unique
        /// - Email: Required, valid email format, must be unique
        /// - Phone: Required, valid phone format
        /// - Description: Optional, 10-1000 characters
        /// - Password: Required, minimum 5 characters
        /// - ConfirmPassword: Required, must match Password
        /// - Whatsapp: Optional
        /// - City: Optional
        /// - Governorate: Optional
        /// - PostalCode: Optional
        /// </remarks>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO request)
        {
            var result = await _accountService.RegisterAsync(request);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        /// <summary>
        /// Login with username or email and password.
        /// Requires email to be verified first.
        /// </summary>
        /// <param name="request">Login credentials including username or email and password.</param>
        /// <response code="200">Login successful. Returns access token and refresh token.</response>
        /// <response code="400">Validation error - invalid input.</response>
        /// <response code="401">Invalid credentials.</response>
        /// <response code="403">Email not verified OR account is deactivated.</response>
        /// <response code="500">Unexpected server error.</response>
        /// <remarks>
        /// Field Constraints:
        /// - UsernameOrEmail: Required, valid username or email
        /// - Password: Required, minimum 5 characters
        /// - RememberMe: Optional, boolean for extended session (default false)
        /// </remarks>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO request)
        {
            var result = await _accountService.LoginAsync(request);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        /// <summary>
        /// Get a new access token using a valid refresh token.
        /// </summary>
        /// <param name="request">Refresh token request containing the refresh token.</param>
        /// <response code="200">Token refreshed successfully. Returns new access token and refresh token.</response>
        /// <response code="400">Validation error - invalid input.</response>
        /// <response code="401">Invalid or expired refresh token.</response>
        /// <response code="500">Unexpected server error.</response>
        /// <remarks>
        /// Field Constraints:
        /// - RefreshToken: Required, valid refresh token string
        /// </remarks>
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenDTO request)
        {
            var result = await _accountService.RefreshTokenAsync(request);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        /// <summary>
        /// Logout the authenticated user and revoke their refresh token.
        /// </summary>
        /// <response code="200">Logout successful. Refresh token revoked.</response>
        /// <response code="401">Unauthorized - invalid or missing authentication token.</response>
        /// <response code="500">Unexpected server error.</response>
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var result = await _accountService.LogoutAsync(userId);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        /// <summary>
        /// Verify email address using the token sent to the user's email.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="token">The verification token sent to the user's email.</param>
        /// <response code="200">Email verified successfully.</response>
        /// <response code="400">Invalid or expired verification token.</response>
        /// <response code="404">User not found.</response>
        /// <response code="500">Unexpected server error.</response>
        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] Guid userId, [FromQuery] string token)
        {
            var result = await _accountService.VerifyEmailAsync(userId, token);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        /// <summary>
        /// Resend the email verification link to the user's email address.
        /// </summary>
        /// <param name="request">Request containing the email address to verify.</param>
        /// <response code="200">Verification email sent successfully.</response>
        /// <response code="400">Validation error - invalid email format.</response>
        /// <response code="404">User not found or already verified.</response>
        /// <response code="500">Unexpected server error.</response>
        /// <remarks>
        /// Field Constraints:
        /// - Email: Required, valid email format
        /// </remarks>
        [HttpPost("resend-verification")]
        public async Task<IActionResult> ResendVerification([FromBody] ResendVerificationRequestDto request)
        {
            var result = await _accountService.ResendVerificationEmailAsync(request.Email);
            return StatusCode((int)result.StatusCode, result.Response);
        }
    }
}