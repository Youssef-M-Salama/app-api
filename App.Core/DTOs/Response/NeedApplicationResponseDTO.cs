using App.Core.Enums;
﻿namespace App.Core.DTOs.Response
{
    /// <summary>
    /// Represents a single need application received by the charity.
    /// Returned in the "applications received" list (GET /charity/applications/received).
    /// </summary>
    public class NeedApplicationResponseDTO
    {
        public Guid NeedApplicationId { get; set; }

        /// <summary>The charity need this application targets.</summary>
        public Guid CharityNeedId { get; set; }

        /// <summary>Product name of the charity need (for display convenience).</summary>
        public string ProductName { get; set; } = string.Empty;

        /// <summary>The donor organization that submitted this application.</summary>
        public Guid DonorOrganizationId { get; set; }

        public string DonorOrganizationName { get; set; } = string.Empty;

        /// <summary>Current status: Pending, Accepted, Rejected.</summary>
        /// <summary>0: Pending, 1: Accepted, 2: Rejected</summary>
        public ApplicationStatus Status { get; set; }

        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Whatsapp { get; set; }
        public string? DonorOraganizationDesctption { get; set; }
        public string? ProductImage { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}