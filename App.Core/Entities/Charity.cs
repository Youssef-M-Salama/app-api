using App.Core.IdentityEntities;

namespace App.Core.Entities
{
    public class Charity
    {
        public Guid CharityId { get; set; }
        public string CharityName { get; set; }  
        public string CharityDescription { get; set; }  
        public bool IsVerified { get; set; }  
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation Properties
        public Guid UserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public ICollection<CharityNeed> CharityNeeds { get; set; }
        public ICollection<OfferApplication> OfferApplications { get; set; }
    }
}