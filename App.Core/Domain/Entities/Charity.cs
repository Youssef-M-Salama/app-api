using App.Core.Domain.IdentityEntities;
using App.Core.Domain.Enums;

namespace App.Core.Domain.Entities
{
    public class Charity
    {
        public Guid CharityId { get; set; }
        public string CharityName { get; set; }  
        public string? CharityDescription { get; set; }  
        public VerificationState VerificationState { get; set; }  
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation Properties
        public Guid UserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public ICollection<CharityNeed> CharityNeeds { get; set; }
        public ICollection<OfferApplication> OfferApplications { get; set; }

        // OPTIONAL INFO
        public string? RegistrationNumber { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public string? HeadquartersAddress { get; set; }
        public string? AuthorizedPersonName { get; set; }
        public string? AuthorizedPersonPosition { get; set; }

        // DOCUMENTS (ALL END WITH Url)
        public string? RegistrationCertificateUrl { get; set; }
        public string? BylawsUrl { get; set; }
        public string? FoundersListUrl { get; set; }
        public string? BoardMembersListUrl { get; set; }
        public string? HeadquartersProofUrl { get; set; }
        public string? DelegationDocumentUrl { get; set; }
    }
}