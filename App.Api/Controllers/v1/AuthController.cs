using App.Core.DTOs.Request;
using App.Core.ServiceContracts;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace App.Api.Controllers.v1
{
    [ApiVersion("1.0")]
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
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO request)
        {
            var result = await _accountService.LoginAsync(request);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        /// <summary>
        /// Get a new access token using a valid refresh token.
        /// </summary>
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenDTO request)
        {
            var result = await _accountService.RefreshTokenAsync(request);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        /// <summary>
        /// Logout the authenticated user and revoke their refresh token.
        /// </summary>
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
        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] Guid userId, [FromQuery] string token)
        {
            var result = await _accountService.VerifyEmailAsync(userId, token);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        /// <summary>
        /// Resend the email verification link to the user's email address.
        /// </summary>
        [HttpPost("resend-verification")]
        public async Task<IActionResult> ResendVerification([FromBody] ResendVerificationRequestDto request)
        {
            var result = await _accountService.ResendVerificationEmailAsync(request.Email);
            return StatusCode((int)result.StatusCode, result.Response);
        }
    }
}