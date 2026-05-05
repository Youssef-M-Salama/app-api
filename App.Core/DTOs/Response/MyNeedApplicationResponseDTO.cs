using App.Core.Enums;

namespace App.Core.DTOs.Response
{
    /// <summary>
    /// Represents a single need application sent by the donor organization.
    /// Returned in the "applications sent" list (GET /donor-organization/need-applications/sent).
    /// </summary>
    public class MyNeedApplicationResponseDTO
    {
        public Guid NeedApplicationId { get; set; }

        /// <summary>The charity need this application targets.</summary>
        public Guid CharityNeedId { get; set; }

        /// <summary>Product name of the charity need (for display convenience).</summary>
        public string ProductName { get; set; } = string.Empty;

        /// <summary>The charity that posted the need.</summary>
        public string CharityName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public MeasurementUnit Unit { get; set; }

        /// <summary>Current status: Pending, Accepted, Rejected.</summary>
        /// <summary>0: Pending, 1: Accepted, 2: Rejected</summary>
        public ApplicationStatus Status { get; set; }

        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Whatsapp { get; set; }
        public string? CharityDescription { get; set; }
        public string? ProductImage { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}