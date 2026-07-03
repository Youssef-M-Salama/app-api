using System.ComponentModel.DataAnnotations;

namespace App.Core.DTOs.Request
{
    public class UpdateProfileRequestDTO
    {
        [RegularExpression(@"^(01[0125][0-9]{8}|1[0125][0-9]{8})$", ErrorMessage = "يرجى إدخال رقم هاتف مصري صحيح (مثال: 1002211457)")]
        public string? Phone { get; set; }

        [RegularExpression(@"^(01[0125][0-9]{8}|1[0125][0-9]{8})$", ErrorMessage = "يرجى إدخال رقم واتساب مصري صحيح (مثال: 1002211457)")]
        public string? Whatsapp { get; set; }

        [StringLength(100, ErrorMessage = "يجب ألا تتجاوز المدينة 100 حرف")]
        public string? City { get; set; }

        [StringLength(100, ErrorMessage = "يجب ألا تتجاوز المحافظة 100 حرف")]
        public string? Governorate { get; set; }

        [StringLength(20, ErrorMessage = "يجب ألا يتجاوز الرمز البريدي 20 حرفاً")]
        public string? PostalCode { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}