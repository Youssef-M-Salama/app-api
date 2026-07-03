using App.Core.Enums;

namespace App.Core.Domain.Entities
{
    public class OfferApplication
    {
        public Guid OfferApplicationId { get; set; }
        public Guid OfferId { get; set; }
        public Guid CharityId { get; set; }
        public ApplicationStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Set when Status transitions to Fulfilled.
        /// Null for Pending, Accepted, and Rejected applications.
        /// </summary>
        public DateTime? FulfillmentDate { get; set; }

        // Navigation Properties
        public Charity Charity { get; set; }
        public Offer Offer { get; set; }
    }
}