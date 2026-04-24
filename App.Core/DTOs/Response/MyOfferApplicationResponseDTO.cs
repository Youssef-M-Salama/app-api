using App.Core.Enums;
﻿namespace App.Core.DTOs.Response
{
    /// <summary>
    /// Represents a single offer application sent by the charity.
    /// Returned in the "applications sent" list (GET /charity/applications/sent).
    /// </summary>
    public class MyOfferApplicationResponseDTO
    {
        public Guid OfferApplicationId { get; set; }

        /// <summary>The offer this application targets.</summary>
        public Guid OfferId { get; set; }

        /// <summary>Product name of the offer (for display convenience).</summary>
        public string ProductName { get; set; } = string.Empty;

        /// <summary>The donor organization that posted the offer.</summary>
        public string DonorOrganizationName {  get; set; } = string.Empty;

        /// <summary>
        /// The Charity thats apply to this offer
        /// </summary>
        public string CharityName { get; set; } = string.Empty;

        /// <summary>Current status: Pending, Accepted, Rejected.</summary>
        /// <summary>0: Pending, 1: Accepted, 2: Rejected</summary>
        public ApplicationStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}