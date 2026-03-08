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
        [HttpGet("requests")]
        //[ProducesResponseType(typeof(ApiResponse<IEnumerable<CharityNeedResponseDto>>), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetApprovedRequests([FromQuery] GetApprovedRequestsQueryDto query)
        {
            var result = await _publicService.GetApprovedRequestsAsync(query);
            return StatusCode((int)result.StatusCode, result.Response);
        }
    }
}