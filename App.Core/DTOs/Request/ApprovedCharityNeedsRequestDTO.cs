using App.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace App.Core.DTOs.Request
{
    /// <summary>
    /// Query parameters for browsing approved charity needs.
    /// </summary>
    public class ApprovedCharityNeedsRequestDTO
    {
        /// <summary>0: Food, 1: Clothing, 2: Medical, 3: Education, 4: Other</summary>
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
        [Range(1, 50, ErrorMessage = "يجب أن يكون حجم الصفحة بين 1 و 50.")]
        public int PageSize { get; set; } = 10;
    }
}