using App.Core.Enums;

namespace App.Core.DTOs.Response
{
    /// <summary>
    /// Authentication response containing user info and JWT token
    /// </summary>
    public class AuthResponseDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        /// <summary>
        /// User role.
        /// 0: Charity, 1: DonorOrganization, 2: Admin
        /// </summary>
        public UserRole Role { get; set; }
        public bool IsVerified { get; set; }
        public string OrganizationName { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public DateTime TokenExpiration { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpiration { get; set; }
    }
}