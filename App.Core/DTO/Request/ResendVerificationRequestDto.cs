using System.ComponentModel.DataAnnotations;

namespace App.Core.DTO.Request
{
    /// <summary>
    /// Request body for resending the email verification link.
    /// </summary>
    public class ResendVerificationRequestDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Email should be in proper format")]
        public string Email { get; set; } = string.Empty;
    }
}