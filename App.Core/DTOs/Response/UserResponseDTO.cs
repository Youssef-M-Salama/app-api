using App.Core.Enums;
using App.Core.Domain.Enums;

namespace App.Core.DTOs.Response
{
    public class UserResponseDTO
    {
        public Guid UserId { get; set; }
        public string?UserName { get; set; }
        public string? Email { get; set; }
        /// <summary>
        /// User role.
        /// 0: Charity, 1: DonorOrganization, 2: Admin
        /// </summary>
        public UserRole? Role { get; set; }
        public bool IsActive { get; set; }
        public VerificationState VerificationState { get; set; }
        public bool VerifyMyAccount { get; set; }
        public string? Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? City { get; set; }
        public string? Governorate { get; set; }
        public string? ImageUrl { get; set; }
        public string? Phone { get; set; }
        public string? Whatsapp { get; set; }
        public string? Description { get; set; }

        public CharityVerificationDataDTO? CharityData { get; set; }
        public DonorVerificationDataDTO? DonorData { get; set; }
    }

    public class CharityVerificationDataDTO
    {
        public string? RegistrationNumber { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public string? HeadquartersAddress { get; set; }
        public string? AuthorizedPersonName { get; set; }
        public string? AuthorizedPersonPosition { get; set; }
        public string? RegistrationCertificateUrl { get; set; }
        public string? BylawsUrl { get; set; }
        public string? FoundersListUrl { get; set; }
        public string? BoardMembersListUrl { get; set; }
        public string? HeadquartersProofUrl { get; set; }
        public string? DelegationDocumentUrl { get; set; }
    }

    public class DonorVerificationDataDTO
    {
        public string? CommercialRegistrationNumber { get; set; }
        public DateTime? CommercialRegistrationDate { get; set; }
        public string? TaxNumber { get; set; }
        public string? BusinessLicenseNumber { get; set; }
        public string? HeadquartersAddress { get; set; }
        public string? CommercialRegisterUrl { get; set; }
        public string? TaxCardUrl { get; set; }
        public string? BusinessLicenseUrl { get; set; }
        public string? CivilProtectionApprovalUrl { get; set; }
        public string? EnvironmentalApprovalUrl { get; set; }
        public string? OwnershipContractUrl { get; set; }
    }
}
