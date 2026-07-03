using App.Core.Enums;
using App.Core.Domain.Enums;

namespace App.Core.DTOs.Response
{
    public class ProfileResponseDTO
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Whatsapp { get; set; }
        public string? City { get; set; }
        public string? Governorate { get; set; }
        public string? PostalCode { get; set; }
        public string? ImageUrl { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        /// <summary>
        /// User role.
        /// 0: Charity, 1: DonorOrganization, 2: Admin
        /// </summary>
        public UserRole Role { get; set; }
        public VerificationState VerificationState { get; set; }
        public bool VerifyMyAccount { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public CharityDetailsDTO? CharityDetails { get; set; }
        public DonorDetailsDTO? DonorDetails { get; set; }
    }
}