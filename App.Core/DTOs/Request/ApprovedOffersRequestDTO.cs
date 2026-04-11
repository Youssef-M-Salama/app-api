using App.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace App.Core.DTOs.Request
{
    /// <summary>
    /// Query parameters for browsing approved offers.
    /// </summary>
    public class ApprovedOffersRequestDTO
    {
        public ProductCategory? Category { get; set; }

        /// <summary>Optional filter by city.</summary>
        public string? City { get; set; }

        /// <summary>Optional filter by governorate.</summary>
        public string? Governorate { get; set; }

        /// <summary>Optional search term matched against product name.</summary>
        public string? Search { get; set; }

        /// <summary>Page number (1-based). Defaults to 1.</summary>
        public int Page { get; set; } = 1;

        /// <summary>Number of items per page. Defaults to 10, max 50.</summary>
        [Range(1, 50, ErrorMessage = "PageSize must be between 1 and 50.")]
        public int PageSize { get; set; } = 10;
    }
}   