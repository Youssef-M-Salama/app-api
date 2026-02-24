using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers.v1
{
    [ApiVersion("1.0")]
    public class TestController : CustomControllerBase
    {
        /// <summary>
        /// Handles HTTP GET requests and returns a simple greeting message.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> containing the string "Hello World" with an HTTP 200 OK status.</returns>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Hello World");
        }
    }
}
