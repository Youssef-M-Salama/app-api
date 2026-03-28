namespace App.Core.DTOs.Request
{
    /// <summary>
    /// Shared pagination parameters used by list endpoints that only need page and pageSize.
    /// </summary>
    public class PaginationFilterDTO
    {
        /// <summary>Page number (1-based). Defaults to 1.</summary>
        public int Page { get; set; } = 1;

        /// <summary>Number of items per page. Defaults to 10, max 50.</summary>
        public int PageSize { get; set; } = 10;
    }
}