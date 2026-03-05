using System.ComponentModel.DataAnnotations;

namespace App.Core.DTO.Request
{
    /// <summary>
    /// Query parameters for browsing approved charity needs.
    /// </summary>
    public class GetApprovedRequestsQueryDto
    {
        /// <summary>
        /// Optional filter by category (food, clothing, medical, education).
        /// </summary>
        public string? Category { get; set; }

        /// <summary>
        /// Optional search term matched against product name.
        /// </summary>
        public string? Search { get; set; }

        /// <summary>
        /// Page number (1-based). Defaults to 1.
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// Number of items per page. Defaults to 10, max 50.
        /// </summary>
        [Range(1, 50, ErrorMessage = "PageSize must be between 1 and 50.")]
        public int PageSize { get; set; } = 10;
    }
}