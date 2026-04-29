using App.Core.Enums;
using System.Net;

namespace App.Core.DTOs.ResultPattern
{
    /// <summary>
    /// Represents a service layer result that wraps the API response
    /// and the HTTP status code to return from the controller.
    ///
    /// <para><b>Controller usage:</b></para>
    /// <code>
    ///     var result = await _service.GetOrderAsync(id);
    ///     return StatusCode((int)result.StatusCode, result.Response);
    /// </code>
    ///
    /// <para><b>Service usage:</b></para>
    /// <code>
    ///     // Return data
    ///     return ServiceResult&lt;OrderDto&gt;.Success("Order retrieved.", order);
    ///
    ///     // Return not found
    ///     return ServiceResult&lt;OrderDto&gt;.NotFound("Order not found.");
    ///
    ///     // Return created resource
    ///     return ServiceResult&lt;OrderDto&gt;.Created("Order created.", order);
    ///
    ///     // Return paginated data
    ///     var pagination = PaginationInfo.Create(page, pageSize, totalCount);
    ///     return ServiceResult&lt;List&lt;OrderDto&gt;&gt;.SuccessPaginated("Orders retrieved.", orders, pagination);
    ///
    ///     // Return typed validation errors
    ///     return ServiceResult&lt;OrderDto&gt;.ValidationError("Validation failed.", errors =>
    ///     {
    ///         errors.Add(new FieldError { Field = "email",    Message = "Invalid format." });
    ///         errors.Add(new FieldError { Field = "password", Message = "Too short." });
    ///     });
    ///
    ///     // Return delete with no content
    ///     return ServiceResult&lt;object&gt;.NoContent();
    /// </code>
    /// </summary>
    public class ServiceResult<T>
    {
        /// <summary>
        /// The structured API response to serialize and return to the client.
        /// </summary>
        public ApiResponse<T> Response { get; }

        /// <summary>
        /// The HTTP status code to set on the response.
        /// </summary>
        public HttpStatusCode StatusCode { get; }

        private ServiceResult(ApiResponse<T> response, HttpStatusCode statusCode)
        {
            Response = response;
            StatusCode = statusCode;
        }

        // =========================================================
        // SUCCESS
        // =========================================================

        /// <summary>
        /// 200 OK — Successful result with data and optional metadata.
        /// </summary>
        public static ServiceResult<T> Success(string message, T data)
        {
            return new ServiceResult<T>(
                new ApiResponse<T>
                {
                    Success = true,
                    Message = message,
                    Data = data
                },
                HttpStatusCode.OK);
        }

        /// <summary>
        /// 200 OK — Successful result without data (e.g. update confirmed).
        /// </summary>
        public static ServiceResult<T> Success(string message)
        {
            return new ServiceResult<T>(
                new ApiResponse<T>
                {
                    Success = true,
                    Message = message
                },
                HttpStatusCode.OK);
        }

        /// <summary>
        /// 200 OK — Successful paginated result.
        /// </summary>
        public static ServiceResult<T> SuccessPaginated(
            string message,
            T data,
            PaginationInfo pagination)
        {
            return new ServiceResult<T>(
                new ApiResponse<T>
                {
                    Success = true,
                    Message = message,
                    Data = data,
                    Pagination = pagination
                },
                HttpStatusCode.OK);
        }

        /// <summary>
        /// 201 Created — Resource was successfully created.
        /// </summary>
        public static ServiceResult<T> Created(string message, T data)
        {
            return new ServiceResult<T>(
                new ApiResponse<T>
                {
                    Success = true,
                    Message = message,
                    Data = data
                },
                HttpStatusCode.Created);
        }
        public static ServiceResult<T> Created(string message)
        {
            return new ServiceResult<T>(
                new ApiResponse<T>
                {
                    Success = true,
                    Message = message
                },
                HttpStatusCode.Created);
        }

        /// <summary>
        /// 204 No Content — Operation succeeded but there is nothing to return.
        /// Typically used for DELETE operations.
        /// </summary>
        public static ServiceResult<T> NoContent(string message = "تمت العملية بنجاح.")
        {
            return new ServiceResult<T>(
                new ApiResponse<T>
                {
                    Success = true,
                    Message = message
                },
                HttpStatusCode.NoContent);
        }

        // =========================================================
        // PAGINATION VALIDATION ERRORS
        // =========================================================

        /// <summary>
        /// 400 Bad Request — Page number is invalid.
        /// </summary>
        public static ServiceResult<T> InvalidPage(object? details = null)
            => Error("رقم الصفحة غير صالح.", ErrorCode.INVALID_PAGE, HttpStatusCode.BadRequest, details);

        /// <summary>
        /// 400 Bad Request — Page size is invalid.
        /// </summary>
        public static ServiceResult<T> InvalidPageSize(object? details = null)
            => Error("حجم الصفحة غير صالح.", ErrorCode.INVALID_PAGE_SIZE, HttpStatusCode.BadRequest, details);

        /// <summary>
        /// 400 Bad Request — Requested page size exceeds the allowed maximum.
        /// </summary>
        public static ServiceResult<T> PageSizeTooLarge(object? details = null)
            => Error("حجم الصفحة يتجاوز الحد المسموح به.", ErrorCode.PAGE_SIZE_LIMIT_EXCEEDED, HttpStatusCode.BadRequest, details);

        // =========================================================
        // CLIENT ERRORS
        // =========================================================

        /// <summary>
        /// 400 Bad Request — Generic validation or bad input error.
        /// Pass a <see cref="ValidationErrorDetails"/> instance as details
        /// for structured field-level errors.
        /// </summary>
        public static ServiceResult<T> BadRequest(string message, object? details = null)
            => Error(message, ErrorCode.VALIDATION_ERROR, HttpStatusCode.BadRequest, details);

        /// <summary>
        /// 400 Bad Request — Structured field validation failure.
        /// Automatically wraps errors in <see cref="ValidationErrorDetails"/>.
        /// <example>
        /// <code>
        /// return ServiceResult&lt;T&gt;.ValidationError("Validation failed.", errors =>
        /// {
        ///     errors.Add(new FieldError { Field = "email",    Message = "Invalid format." });
        ///     errors.Add(new FieldError { Field = "password", Message = "Too short." });
        /// });
        /// </code>
        /// </example>
        /// </summary>
        public static ServiceResult<T> ValidationError(
            string message,
            Action<List<FieldError>> buildErrors)
        {
            var details = new ValidationErrorDetails();
            buildErrors(details.Errors);
            return Error(message, ErrorCode.VALIDATION_ERROR, HttpStatusCode.BadRequest, details);
        }

        /// <summary>
        /// 401 Unauthorized — Request is missing valid authentication credentials.
        /// </summary>
        public static ServiceResult<T> Unauthorized(string message)
            => Error(message, ErrorCode.UNAUTHORIZED, HttpStatusCode.Unauthorized);

        /// <summary>
        /// 403 Forbidden — Caller is authenticated but lacks required permission.
        /// </summary>
        public static ServiceResult<T> Forbidden(string message)
            => Error(message, ErrorCode.FORBIDDEN, HttpStatusCode.Forbidden);

        /// <summary>
        /// 404 Not Found — The requested resource does not exist.
        /// </summary>
        public static ServiceResult<T> NotFound(string message)
            => Error(message, ErrorCode.NOT_FOUND, HttpStatusCode.NotFound);

        /// <summary>
        /// 409 Conflict — Resource already exists or state conflict detected.
        /// </summary>
        public static ServiceResult<T> Conflict(string message, object? details = null)
            => Error(message, ErrorCode.ALREADY_EXISTS, HttpStatusCode.Conflict, details);

        // =========================================================
        // SERVER ERRORS
        // =========================================================

        /// <summary>
        /// 500 Internal Server Error — An unexpected server-side error occurred.
        /// Avoid exposing sensitive details in production.
        /// </summary>
        public static ServiceResult<T> Internal(string message, object? details = null)
            => Error(message, ErrorCode.INTERNAL_SERVER_ERROR, HttpStatusCode.InternalServerError, details);

        // =========================================================
        // CORE ERROR BUILDER
        // =========================================================

        /// <summary>
        /// Core error factory. All named error methods delegate here.
        /// Use this directly only when no named method fits your case.
        /// </summary>
        public static ServiceResult<T> Error(
            string message,
            ErrorCode code,
            HttpStatusCode status,
            object? details = null)
        {
            return new ServiceResult<T>(
                new ApiResponse<T>
                {
                    Success = false,
                    Message = message,
                    Error = new ErrorInfo
                    {
                        Code = code.ToString(),
                        Details = details
                    }
                },
                status);
        }
    }
} 