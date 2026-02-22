namespace App.Core.Entities
{
    public class OfferApplication
    {
        public Guid OfferApplicationId { get; set; }
        public Guid OfferId { get; set; }
        public Guid CharityId { get; set; }  
        public string Status { get; set; }  // 'pending', 'accepted', 'rejected'
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation Properties
        public Charity Charity { get; set; }
        public Offer Offer { get; set; }
    }
}