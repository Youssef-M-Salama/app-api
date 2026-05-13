using Microsoft.AspNetCore.Http;

namespace App.Core.DTOs.Request
{
    public class UpdateCharityVerificationDataRequestDTO
    {
        public string? RegistrationNumber { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public string? HeadquartersAddress { get; set; }
        public string? AuthorizedPersonName { get; set; }
        public string? AuthorizedPersonPosition { get; set; }

        public IFormFile? RegistrationCertificate { get; set; }
        public IFormFile? Bylaws { get; set; }
        public IFormFile? FoundersList { get; set; }
        public IFormFile? BoardMembersList { get; set; }
        public IFormFile? HeadquartersProof { get; set; }
        public IFormFile? DelegationDocument { get; set; }
    }
}
