using App.Core.DTO.Response;
using App.Core.Enums;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers.v1
{
    /// <summary>
    /// Test controller demonstrating all possible API response types.
    /// Use this as a reference when building real controllers.
    /// </summary>
    [ApiVersion("1.0")]
    public class TestController : CustomControllerBase
    {
        // =========================================================
        // SUCCESS RESPONSES
        // =========================================================

        /// <summary>
        /// 200 OK — Returns data.
        /// </summary>
        [HttpGet("success")]
        public IActionResult GetSuccess()
        {
            var result = ServiceResult<string>.Success(
                "Data retrieved successfully.",
                "Hello World"
            );

            return StatusCode((int)result.StatusCode, result.Response);
        }

        /// <summary>
        /// 200 OK — Returns no data (e.g. after an update).
        /// </summary>
        [HttpGet("success-no-data")]
        public IActionResult GetSuccessNoData()
        {
            var result = ServiceResult<object>.Success("Operation completed.");

            return StatusCode((int)result.StatusCode, result.Response);
        }

        /// <summary>
        /// 200 OK — Returns paginated data.
        /// </summary>
        [HttpGet("success-paginated")]
        public IActionResult GetPaginated()
        {
            var data = new List<string> { "Item 1", "Item 2", "Item 3" };
            var pagination = PaginationInfo.Create(page: 1, pageSize: 10, totalCount: 3);

            var result = ServiceResult<List<string>>.SuccessPaginated(
                "Items retrieved successfully.",
                data,
                pagination
            );

            return StatusCode((int)result.StatusCode, result.Response);
        }

        /// <summary>
        /// 201 Created — Resource created successfully.
        /// </summary>
        [HttpPost("created")]
        public IActionResult PostCreated()
        {
            var created = new { Id = 1, Name = "New Resource" };

            var result = ServiceResult<object>.Created(
                "Resource created successfully.",
                created
            );

            return StatusCode((int)result.StatusCode, result.Response);
        }

        /// <summary>
        /// 204 No Content — Delete succeeded, nothing to return.
        /// </summary>
        [HttpDelete("no-content")]
        public IActionResult DeleteNoContent()
        {
            var result = ServiceResult<object>.NoContent();

            return StatusCode((int)result.StatusCode, result.Response);
        }

        // =========================================================
        // CLIENT ERROR RESPONSES
        // =========================================================

        /// <summary>
        /// 400 Bad Request — Generic bad input.
        /// </summary>
        [HttpGet("bad-request")]
        public IActionResult GetBadRequest()
        {
            var result = ServiceResult<object>.BadRequest(
                "The request contains invalid data."
            );

            return StatusCode((int)result.StatusCode, result.Response);
        }

        /// <summary>
        /// 400 Bad Request — Typed field-level validation errors.
        /// </summary>
        [HttpGet("validation-error")]
        public IActionResult GetValidationError()
        {
            var result = ServiceResult<object>.ValidationError(
                "Validation failed.",
                errors =>
                {
                    errors.Add(new FieldError { Field = "email", Message = "Invalid email format." });
                    errors.Add(new FieldError { Field = "password", Message = "Must be at least 8 characters." });
                }
            );

            return StatusCode((int)result.StatusCode, result.Response);
        }

        /// <summary>
        /// 400 Bad Request — Invalid page number.
        /// </summary>
        [HttpGet("invalid-page")]
        public IActionResult GetInvalidPage()
        {
            var result = ServiceResult<object>.InvalidPage();

            return StatusCode((int)result.StatusCode, result.Response);
        }

        /// <summary>
        /// 400 Bad Request — Invalid page size.
        /// </summary>
        [HttpGet("invalid-page-size")]
        public IActionResult GetInvalidPageSize()
        {
            var result = ServiceResult<object>.InvalidPageSize();

            return StatusCode((int)result.StatusCode, result.Response);
        }

        /// <summary>
        /// 400 Bad Request — Page size exceeds allowed limit.
        /// </summary>
        [HttpGet("page-size-too-large")]
        public IActionResult GetPageSizeTooLarge()
        {
            var result = ServiceResult<object>.PageSizeTooLarge();

            return StatusCode((int)result.StatusCode, result.Response);
        }

        /// <summary>
        /// 401 Unauthorized — No valid authentication credentials.
        /// </summary>
        [HttpGet("unauthorized")]
        public IActionResult GetUnauthorized()
        {
            var result = ServiceResult<object>.Unauthorized(
                "Authentication is required to access this resource."
            );

            return StatusCode((int)result.StatusCode, result.Response);
        }

        /// <summary>
        /// 403 Forbidden — Authenticated but lacks permission.
        /// </summary>
        [HttpGet("forbidden")]
        public IActionResult GetForbidden()
        {
            var result = ServiceResult<object>.Forbidden(
                "You do not have permission to perform this action."
            );

            return StatusCode((int)result.StatusCode, result.Response);
        }

        /// <summary>
        /// 404 Not Found — Requested resource does not exist.
        /// </summary>
        [HttpGet("not-found")]
        public IActionResult GetNotFound()
        {
            var result = ServiceResult<object>.NotFound(
                "The requested resource was not found."
            );

            return StatusCode((int)result.StatusCode, result.Response);
        }

        /// <summary>
        /// 409 Conflict — Resource already exists.
        /// </summary>
        [HttpGet("conflict")]
        public IActionResult GetConflict()
        {
            var result = ServiceResult<object>.Conflict(
                "A resource with this identifier already exists."
            );

            return StatusCode((int)result.StatusCode, result.Response);
        }

        // =========================================================
        // SERVER ERROR RESPONSES
        // =========================================================

        /// <summary>
        /// 500 Internal Server Error — Unexpected server-side failure.
        /// </summary>
        [HttpGet("internal-error")]
        public IActionResult GetInternalError()
        {
            var result = ServiceResult<object>.Internal(
                "An unexpected error occurred. Please try again later."
            );

            return StatusCode((int)result.StatusCode, result.Response);
        }

        // =========================================================
        // CUSTOM ERROR (escape hatch)
        // =========================================================

        /// <summary>
        /// Custom error — use when no named factory method fits.
        /// </summary>
        [HttpGet("custom-error")]
        public IActionResult GetCustomError()
        {
            var result = ServiceResult<object>.Error(
                "Something specific went wrong.",
                ErrorCode.VALIDATION_ERROR,
                System.Net.HttpStatusCode.BadRequest,
                new { Reason = "Custom error detail here." }
            );

            return StatusCode((int)result.StatusCode, result.Response);
        }
    }
}