using System.ComponentModel.DataAnnotations;

namespace App.Core.DTOs.Request
{
    /// <summary>
    /// Request body for refreshing an expired access token.
    /// </summary>
    public class RefreshTokenDTO
    {
        [Required(ErrorMessage = "رمز التحديث مطلوب")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}