using System.ComponentModel.DataAnnotations;

namespace App.Core.DTOs.Request
{
    public class ChangePasswordRequestDTO
    {
        [Required(ErrorMessage = "كلمة المرور الحالية مطلوبة")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "كلمة المرور الجديدة مطلوبة")]
        [MinLength(5, ErrorMessage = "يجب أن تتكون كلمة المرور من 5 أحرف على الأقل")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "تأكيد كلمة المرور مطلوب")]
        [Compare(nameof(NewPassword), ErrorMessage = "كلمات المرور غير متطابقة")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}