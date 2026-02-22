using App.Core.IdentityEntities;

namespace App.Core.Entities
{
    public class DonorOrganization  
    {
        public Guid DonorId { get; set; }
        public string DonorName { get; set; }
        public string? DonorOrganizationImage { get; set; } 
        public bool IsVerified { get; set; }  
        public bool IsActive { get; set; }  
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation Properties
        public Guid UserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public ICollection<Offer> Offers { get; set; }
        public ICollection<NeedApplication> NeedApplications { get; set; }
    }
}