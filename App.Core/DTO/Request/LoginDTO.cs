using System.ComponentModel.DataAnnotations;

namespace App.Core.DTO.Request
{
    /// <summary>
    /// Login request data
    /// </summary>
    public class LoginDTO
    {
        [Required(ErrorMessage = "Username or email is required")]
        public string UsernameOrEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }
}