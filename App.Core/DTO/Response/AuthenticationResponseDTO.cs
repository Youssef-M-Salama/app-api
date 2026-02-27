namespace App.Core.DTO.Response
{
    /// <summary>
    /// Authentication response containing user info and JWT token
    /// </summary>
    public class AuthResponseDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsVerified { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime TokenExpiration { get; set; }
    }
}