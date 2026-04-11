using App.Core.Enums;

namespace App.Core.DTOs.Response
{
    public class UserResponseDTO
    {
        public Guid UserId { get; set; }
        public string? Email { get; set; }
        /// <summary>
        /// User role.
        /// 0: Admin, 1: Charity, 2: DonorOrganization
        /// </summary>
        public UserRole? Role { get; set; }
        public bool IsActive { get; set; }
        public bool IsVerified { get; set; }
        public string? Name { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
