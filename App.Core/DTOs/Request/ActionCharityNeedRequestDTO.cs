using System.ComponentModel.DataAnnotations;

namespace App.Core.DTOs.Request
{
    public class ActionCharityNeedRequestDTO
    {
        [Required(ErrorMessage = "CharityNeedId is required")]
        public Guid CharityNeedId { get; set; }
    }
}
