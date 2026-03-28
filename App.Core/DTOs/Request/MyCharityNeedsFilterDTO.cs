namespace App.Core.DTOs.Request
{
    /// <summary>
    /// Query parameters for browsing the authenticated charity's own needs.
    /// </summary>
    public class MyCharityNeedsFilterDTO
    {
        /// <summary>Optional filter by status (Pending, Approved, Rejected, Fulfilled).</summary>
        public string? Status { get; set; }

        /// <summary>Page number (1-based). Defaults to 1.</summary>
        public int Page { get; set; } = 1;

        /// <summary>Number of items per page. Defaults to 10, max 50.</summary>
        public int PageSize { get; set; } = 10;
    }
}