namespace App.Core.DTO.Response
{
    /// <summary>
    /// Standard API response wrapper used for all endpoints.
    /// Provides consistent success, data, metadata, and error structure.
    /// </summary>
    /// <typeparam name="T">Type of response data.</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Indicates whether the request was successful.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Human readable message describing the result.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Payload returned from the API.
        /// </summary>
        public T? Data { get; set; }
        /// <summary>
        /// Pagination information when endpoint returns paginated data.
        /// </summary>
        public PaginationInfo? Pagination { get; set; }

        /// <summary>
        /// Optional metadata (pagination, extra info, etc.).
        /// </summary>
        public object? Meta { get; set; }

        /// <summary>
        /// Error information when request fails.
        /// </summary>
        public ErrorInfo? Error { get; set; }
    }

    /// <summary>
    /// Non-generic API response when no data is returned.
    /// </summary>
    public class ApiResponse : ApiResponse<object> { }

    /// <summary>
    /// Represents detailed error information.
    /// </summary>
    public class ErrorInfo
    {
        /// <summary>
        /// Machine readable error code.
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Optional extra error details (validation errors, debug info, etc.).
        /// </summary>
        public object? Details { get; set; }
    }
}