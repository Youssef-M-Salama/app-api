using App.Core.Enums;

namespace App.Core.Domain.Entities
{
    public class NeedApplication
    {
        public Guid NeedApplicationId { get; set; }
        public Guid DonorOrganizationId { get; set; }
        public Guid CharityNeedId { get; set; }
        public ApplicationStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Set when Status transitions to Fulfilled.
        /// Null for Pending, Accepted, and Rejected applications.
        /// </summary>
        public DateTime? FulfillmentDate { get; set; }

        // Navigation Properties
        public DonorOrganization DonorOrganization { get; set; }
        public CharityNeed CharityNeed { get; set; }
    }
}