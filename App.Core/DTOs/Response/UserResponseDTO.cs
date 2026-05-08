using App.Core.Enums;

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
        public bool IsVerified { get; set; }
        public string? Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? City { get; set; }
        public string? Governorate { get; set; }
        public string? ImageUrl { get; set; }
        public string? Phone { get; set; }
        public string? Whatsapp { get; set; }
        public string? Description { get; set; }

    }
}
