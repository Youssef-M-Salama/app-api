using System.Diagnostics;

namespace App.Core.DTO.Response
{
    /// <summary>
    /// Standard API response wrapper used for all endpoints.
    /// Provides consistent success, data, pagination, error, and debug structure.
    /// </summary>
    /// <typeparam name="T">Type of response data.</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Indicates whether the request was successful.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Human-readable message describing the result.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Payload returned from the API.
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// Pagination metadata when the endpoint returns paginated data.
        /// Null for non-paginated responses.
        /// </summary>
        public PaginationInfo? Pagination { get; set; }

        /// <summary>
        /// Structured error information when the request fails.
        /// Null on success.
        /// </summary>
        public ErrorInfo? Error { get; set; }

        /// <summary>
        /// UTC timestamp of when this response was generated.
        /// Useful for debugging, caching decisions, and audit trails.
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

       
    }

    /// <summary>
    /// Non-generic API response for endpoints that return no data (e.g. DELETE).
    /// </summary>
    public class ApiResponse : ApiResponse<object?> { }

    /// <summary>
    /// Represents structured error information returned on failure.
    /// </summary>
    public class ErrorInfo
    {
        /// <summary>
        /// Machine-readable error code (maps to <see cref="App.Core.Enums.ErrorCode"/>).
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Optional extra error details.
        /// For validation errors, this will be a list of <see cref="FieldError"/>.
        /// For other errors, this may contain debug information.
        /// </summary>
        public object? Details { get; set; }
    }

    /// <summary>
    /// Represents a single field-level validation error.
    /// Used inside <see cref="ErrorInfo.Details"/> for validation failures.
    /// </summary>
    public class FieldError
    {
        /// <summary>
        /// The name of the field that failed validation.
        /// </summary>
        public string Field { get; set; } = string.Empty;

        /// <summary>
        /// Human-readable description of why the field failed.
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }

    /// <summary>
    /// Typed container for validation error details.
    /// Passed as <see cref="ErrorInfo.Details"/> when multiple field errors exist.
    /// </summary>
    public class ValidationErrorDetails
    {
        /// <summary>
        /// List of all field-level validation errors.
        /// </summary>
        public List<FieldError> Errors { get; set; } = new();
    }
}