using System.ComponentModel.DataAnnotations;

namespace App.Core.DTO.Request
{
    /// <summary>
    /// Request body for refreshing an expired access token.
    /// </summary>
    public class RefreshTokenDTO
    {
        [Required(ErrorMessage = "Refresh token is required")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}