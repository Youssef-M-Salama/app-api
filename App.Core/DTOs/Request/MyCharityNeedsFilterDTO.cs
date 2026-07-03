using App.Core.Enums;

namespace App.Core.DTOs.Request
{
    /// <summary>
    /// Query parameters for browsing the authenticated charity's own needs.
    /// </summary>
    public class MyCharityNeedsFilterDTO
    {
        /// <summary>0: Pending, 1: Approved, 2: Rejected, 3: Fulfilled</summary>
        public CharityNeedStatus? Status { get; set; }

        /// <summary>Page number (1-based). Defaults to 1.</summary>
        public int Page { get; set; } = 1;

        /// <summary>Number of items per page. Defaults to 10, max 50.</summary>
        public int PageSize { get; set; } = 10;
    }
}