using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace App.Core.DTOs.Request
{
    public class UpdateProfileImageRequestDTO
    {
        [Required(ErrorMessage = "Image is required")]
        public IFormFile Image { get; set; } = null!;
    }
}