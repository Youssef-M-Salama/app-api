namespace App.Core.Entities
{
    public class Offer
    {
        public Guid OfferId { get; set; }
        public Guid DonorOrganizationId { get; set; }  
        public Guid? AdminId { get; set; }  
        public string Category { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }  
        public string? ProductImage { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string Status { get; set; }  
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation Properties
        public DonorOrganization DonorOrganization { get; set; } 
        public ICollection<OfferApplication> OfferApplications { get; set; }
    }
}