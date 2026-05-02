using App.Core.Domain.IdentityEntities;

namespace App.Core.Domain.Entities
{
    public class DonorOrganization  
    {
        public Guid DonorOrganizationId { get; set; }
        public string DonorOrganizationName { get; set; }
        public string? DonorOrganizationDescription { get; set; }

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