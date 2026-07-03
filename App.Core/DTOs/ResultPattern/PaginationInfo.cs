namespace App.Core.DTOs.ResultPattern
{
    /// <summary>
    /// Represents pagination metadata returned with paginated API responses.
    /// Contains current paging state and navigation flags.
    /// </summary>
    public class PaginationInfo
    {
        /// <summary>
        /// Current page number (1-based).
        /// </summary>
        public int Page { get; init; }

        /// <summary>
        /// Number of items requested per page.
        /// </summary>
        public int PageSize { get; init; }

        /// <summary>
        /// Total number of available pages.
        /// </summary>
        public int TotalPages { get; init; }

        /// <summary>
        /// Total number of items across all pages.
        /// </summary>
        public int TotalCount { get; init; }

        /// <summary>
        /// Indicates whether a next page exists.
        /// </summary>
        public bool HasNext { get; init; }

        /// <summary>
        /// Indicates whether a previous page exists.
        /// </summary>
        public bool HasPrevious { get; init; }

        /// <summary>
        /// Creates pagination metadata from paging inputs.
        /// All inputs are normalized to safe values before calculation.
        /// </summary>
        /// <param name="page">Requested page number (1-based). Normalized to 1 if below 1.</param>
        /// <param name="pageSize">Requested page size. Normalized to 10 if below 1.</param>
        /// <param name="totalCount">Total number of items. Normalized to 0 if negative.</param>
        /// <returns>A fully computed <see cref="PaginationInfo"/> instance.</returns>
        public static PaginationInfo Create(int page, int pageSize, int totalCount)
        {
            if (pageSize <= 0) pageSize = 10;
            if (page <= 0) page = 1;
            if (totalCount < 0) totalCount = 0;

            var totalPages = totalCount == 0
                ? 0
                : (int)Math.Ceiling(totalCount / (double)pageSize);

            return new PaginationInfo
            {
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages,
                TotalCount = totalCount,
                HasNext = page < totalPages,
                HasPrevious = page > 1
            };
        }
    }
}