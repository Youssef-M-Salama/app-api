using System.ComponentModel.DataAnnotations;

namespace App.Core.DTOs.Request
{
    public class ActionOfferRequestDTO
    {
        [Required(ErrorMessage = "معرف العرض مطلوب")]
        public Guid OfferId { get; set; }
    }
}
