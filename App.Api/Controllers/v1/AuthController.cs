using App.Core.DTO.Request;
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
        /// Register a new charity or donor organization account
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO request)
        {
            var result = await _accountService.RegisterAsync(request);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        /// <summary>
        /// Login with username or email and password
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO request)
        {
            var result = await _accountService.LoginAsync(request);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        /// <summary>
        /// Get a new access token using a valid refresh token
        /// </summary>
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenDTO request)
        {
            var result = await _accountService.RefreshTokenAsync(request);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        /// <summary>
        /// Logout the authenticated user and revoke their refresh token
        /// </summary>
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            // Extract user ID from the JWT claims
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var result = await _accountService.LogoutAsync(userId);
            return StatusCode((int)result.StatusCode, result.Response);
        }
    }
}