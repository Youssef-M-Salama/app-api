using System.ComponentModel.DataAnnotations;

namespace App.Core.DTOs.Request
{
    public class ActionUserRequestDTO
    {
        [Required(ErrorMessage = "معرف المستخدم مطلوب")]
        public Guid UserId { get; set; }
    }
}
