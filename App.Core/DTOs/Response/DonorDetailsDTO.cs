namespace App.Core.DTOs.Response
{
    public class DonorDetailsDTO
    {
        public string DonorOrganizationName { get; set; } = string.Empty;
        public string? DonorOrganizationDescription { get; set; }
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