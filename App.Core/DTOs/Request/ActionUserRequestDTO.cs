using System.ComponentModel.DataAnnotations;

namespace App.Core.DTOs.Request
{
    public class ActionUserRequestDTO
    {
        [Required(ErrorMessage = "UserId is required")]
        public Guid UserId { get; set; }
    }
}
