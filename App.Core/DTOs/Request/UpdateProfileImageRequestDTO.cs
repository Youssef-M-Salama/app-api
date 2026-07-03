using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace App.Core.DTOs.Request
{
    public class UpdateProfileImageRequestDTO
    {
        [Required(ErrorMessage = "الصورة مطلوبة")]
        public IFormFile Image { get; set; } = null!;
    }
}