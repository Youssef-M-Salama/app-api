using System.ComponentModel.DataAnnotations;

namespace App.Core.DTOs.Request
{
    /// <summary>
    /// Login request data
    /// </summary>
    public class LoginDTO
    {
        [Required(ErrorMessage = "اسم المستخدم أو البريد الإلكتروني مطلوب")]
        public string UsernameOrEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "كلمة المرور مطلوبة")]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }
}