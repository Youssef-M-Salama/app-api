using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers.v1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/test-security")]
    public class TestSecurityController : CustomControllerBase
    {
        [HttpGet]
        [Authorize]
        public IActionResult Get()
        {
            return Ok("This is a test endpoint to verify security configuration.");
        }

        [HttpPost("test-upload")]
        public async Task<IActionResult> TestUpload(IFormFile file, [FromServices] App.Core.ServiceContracts.IFileService fileService)
        {
            try 
            {
                var result = await fileService.SaveImageAsync(file, App.Core.Enums.ImageFolder.Needs);
                return Ok(result);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }
    }
}
