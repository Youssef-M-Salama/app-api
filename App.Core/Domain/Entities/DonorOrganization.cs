using App.Core.Domain.IdentityEntities;
using App.Core.Domain.Enums;

namespace App.Core.Domain.Entities
{
    public class DonorOrganization  
    {
        public Guid DonorOrganizationId { get; set; }
        public string DonorOrganizationName { get; set; }
        public string? DonorOrganizationDescription { get; set; }

        public VerificationState VerificationState { get; set; }  
        public bool IsActive { get; set; }  
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation Properties
        public Guid UserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public ICollection<Offer> Offers { get; set; }
        public ICollection<NeedApplication> NeedApplications { get; set; }

        // OPTIONAL INFO
        public string? CommercialRegistrationNumber { get; set; }
        public DateTime? CommercialRegistrationDate { get; set; }
        public string? TaxNumber { get; set; }
        public string? BusinessLicenseNumber { get; set; }
        public string? HeadquartersAddress { get; set; }

        // DOCUMENTS (ALL END WITH Url)
        public string? CommercialRegisterUrl { get; set; }
        public string? TaxCardUrl { get; set; }
        public string? BusinessLicenseUrl { get; set; }
        public string? CivilProtectionApprovalUrl { get; set; }
        public string? EnvironmentalApprovalUrl { get; set; }
        public string? OwnershipContractUrl { get; set; }
    }
}