using App.Core.Enums;
﻿namespace App.Core.DTOs.Response
{
    /// <summary>
    /// Represents a single need application sent by the donor organization.
    /// Returned in the "applications sent" list (GET /donor/applications/sent).
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

        /// <summary>Current status: Pending, Accepted, Rejected.</summary>
        /// <summary>0: Pending, 1: Accepted, 2: Rejected</summary>
        public ApplicationStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}