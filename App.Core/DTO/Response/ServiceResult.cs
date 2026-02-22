using App.Core.Enums;
using System.Net;

namespace App.Core.DTO.Response
{
    /// <summary>
    /// Represents a service layer result that includes the API response
    /// and the HTTP status code to return from the controller.
    /// </summary>
    public class ServiceResult<T>
    {
        public ApiResponse<T> Response { get; }
        public HttpStatusCode StatusCode { get; }

        private ServiceResult(ApiResponse<T> response, HttpStatusCode statusCode)
        {
            Response = response;
            StatusCode = statusCode;
        }

        // =========================
        // SUCCESS
        // =========================

        /// <summary>
        /// Successful result with data.
        /// </summary>
        public static ServiceResult<T> Success(string message, T data, object? meta = null)
        {
            return new ServiceResult<T>(
                new ApiResponse<T>
                {
                    Success = true,
                    Message = message,
                    Data = data,
                    Meta = meta
                },
                HttpStatusCode.OK);
        }

        /// <summary>
        /// Successful result without data.
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
        /// Successful paginated result.
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
        /// Created (201) result.
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

        // =========================
        // PAGINATION VALIDATION
        // =========================

        public static ServiceResult<T> InvalidPage(object? details = null)
            => Error("Invalid page.", ErrorCode.INVALID_PAGE, HttpStatusCode.BadRequest, details);

        public static ServiceResult<T> InvalidPageSize(object? details = null)
            => Error("Invalid page size.", ErrorCode.INVALID_PAGE_SIZE, HttpStatusCode.BadRequest, details);

        public static ServiceResult<T> PageSizeTooLarge(object? details = null)
            => Error("Page size too large.", ErrorCode.PAGE_SIZE_LIMIT_EXCEEDED, HttpStatusCode.BadRequest, details);

        // =========================
        // GENERIC ERRORS
        // =========================

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

        public static ServiceResult<T> BadRequest(string message, object? details = null)
            => Error(message, ErrorCode.VALIDATION_ERROR, HttpStatusCode.BadRequest, details);

        public static ServiceResult<T> Unauthorized(string message)
            => Error(message, ErrorCode.UNAUTHORIZED, HttpStatusCode.Unauthorized);

        public static ServiceResult<T> Forbidden(string message)
            => Error(message, ErrorCode.FORBIDDEN, HttpStatusCode.Forbidden);

        public static ServiceResult<T> NotFound(string message)
            => Error(message, ErrorCode.NOT_FOUND, HttpStatusCode.NotFound);

        public static ServiceResult<T> Conflict(string message, object? details = null)
            => Error(message, ErrorCode.ALREADY_EXISTS, HttpStatusCode.Conflict, details);

        public static ServiceResult<T> Internal(string message, object? details = null)
            => Error(message, ErrorCode.INTERNAL_SERVER_ERROR, HttpStatusCode.InternalServerError, details);
    }
}