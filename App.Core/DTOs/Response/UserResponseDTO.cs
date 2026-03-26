namespace App.Core.DTOs.Response
{
    public class UserResponseDTO
    {
        public Guid UserId { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public bool IsActive { get; set; }
        public bool IsVerified { get; set; }
        public string? Name { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
