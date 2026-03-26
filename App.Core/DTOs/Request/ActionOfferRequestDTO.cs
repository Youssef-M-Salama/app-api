using System.ComponentModel.DataAnnotations;

namespace App.Core.DTOs.Request
{
    public class ActionOfferRequestDTO
    {
        [Required(ErrorMessage = "OfferId is required")]
        public Guid OfferId { get; set; }
    }
}
