using App.Core.Enums;

namespace App.Core.DTOs.Request
{
    /// <summary>
    /// Query parameters for browsing the authenticated donor's own offers.
    /// </summary>
    public class MyOffersFilterDTO
    {
        /// <summary>0: Pending, 1: Approved, 2: Rejected, 3: Expired, 4: Fulfilled</summary>
        public OfferStatus? Status { get; set; }

        /// <summary>Page number (1-based). Defaults to 1.</summary>
        public int Page { get; set; } = 1;

        /// <summary>Number of items per page. Defaults to 10, max 50.</summary>
        public int PageSize { get; set; } = 10;
    }
}