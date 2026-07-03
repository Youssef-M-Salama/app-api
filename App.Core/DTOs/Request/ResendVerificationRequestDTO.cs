using System.ComponentModel.DataAnnotations;

namespace App.Core.DTOs.Request
{
    /// <summary>
    /// Request body for resending the email verification link.
    /// </summary>
    public class ResendVerificationRequestDto
    {
        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "يجب أن يكون البريد الإلكتروني بصيغة صحيحة")]
        public string Email { get; set; } = string.Empty;
    }
}