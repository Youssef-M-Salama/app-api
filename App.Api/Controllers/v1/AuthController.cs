using App.Core.DTO.Request;
using App.Core.DTO.Response;
using App.Core.ServiceContracts;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponseDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Register([FromBody] RegisterDTO request)
        {
            var result = await _accountService.RegisterAsync(request);
            return StatusCode((int)result.StatusCode, result.Response);
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDto), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Login([FromBody] LoginDTO request)
        {
            var result = await _accountService.LoginAsync(request);
            return StatusCode((int)result.StatusCode, result.Response);
        }
    }
}