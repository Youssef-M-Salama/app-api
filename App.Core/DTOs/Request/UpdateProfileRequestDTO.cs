using System.ComponentModel.DataAnnotations;

namespace App.Core.DTOs.Request
{
    public class UpdateProfileRequestDTO
    {
        [Phone(ErrorMessage = "صيغة رقم الهاتف غير صالحة")]
        public string? Phone { get; set; }

        public string? Whatsapp { get; set; }

        [StringLength(100, ErrorMessage = "يجب ألا تتجاوز المدينة 100 حرف")]
        public string? City { get; set; }

        [StringLength(100, ErrorMessage = "يجب ألا تتجاوز المحافظة 100 حرف")]
        public string? Governorate { get; set; }

        [StringLength(20, ErrorMessage = "يجب ألا يتجاوز الرمز البريدي 20 حرفاً")]
        public string? PostalCode { get; set; }
    }
}