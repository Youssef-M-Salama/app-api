using App.Core.Enums;
﻿namespace App.Core.DTOs.Response
{
    /// <summary>
    /// Represents a single offer application received by the donor organization.
    /// Returned in the "applications received" list (GET /donor/applications/received).
    /// </summary>
    public class OfferApplicationResponseDTO
    {
        public Guid OfferApplicationId { get; set; }

        /// <summary>The offer this application targets.</summary>
        public Guid OfferId { get; set; }

        /// <summary>Product name of the offer (for display convenience).</summary>
        public string ProductName { get; set; } = string.Empty;

        /// <summary>The charity that submitted this application.</summary>
        public Guid CharityId { get; set; }

        public string CharityName { get; set; } = string.Empty;

        /// <summary>Current status: Pending, Accepted, Rejected.</summary>
        /// <summary>0: Pending, 1: Accepted, 2: Rejected</summary>
        public ApplicationStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}