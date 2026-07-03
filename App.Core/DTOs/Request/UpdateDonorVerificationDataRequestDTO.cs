using Microsoft.AspNetCore.Http;

namespace App.Core.DTOs.Request
{
    public class UpdateDonorVerificationDataRequestDTO
    {
        public string? CommercialRegistrationNumber { get; set; }
        public DateTime? CommercialRegistrationDate { get; set; }
        public string? TaxNumber { get; set; }
        public string? BusinessLicenseNumber { get; set; }
        public string? HeadquartersAddress { get; set; }

        public IFormFile? CommercialRegister { get; set; }
        public IFormFile? TaxCard { get; set; }
        public IFormFile? BusinessLicense { get; set; }
        public IFormFile? CivilProtectionApproval { get; set; }
        public IFormFile? EnvironmentalApproval { get; set; }
        public IFormFile? OwnershipContract { get; set; }
    }
}
