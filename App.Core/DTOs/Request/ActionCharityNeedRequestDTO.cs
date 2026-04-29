using System.ComponentModel.DataAnnotations;

namespace App.Core.DTOs.Request
{
    public class ActionCharityNeedRequestDTO
    {
        [Required(ErrorMessage = "معرف احتياج الجمعية مطلوب")]
        public Guid CharityNeedId { get; set; }
    }
}
