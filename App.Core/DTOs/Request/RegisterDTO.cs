using App.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace App.Core.DTOs.Request
{
    public class RegisterDTO
    {
        /// <summary>0: Charity, 1: DonorOrganization</summary>
        [Required(ErrorMessage = "نوع الحساب مطلوب")]
        public AccountType AccountType { get; set; }

        [Required(ErrorMessage = "لا يمكن أن يكون الاسم فارغاً")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "يجب أن يكون الاسم بين 3 و 200 حرف")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "لا يمكن أن يكون اسم المستخدم فارغاً")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "يجب أن يكون اسم المستخدم بين 3 و 50 حرفاً")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "لا يمكن أن يكون البريد الإلكتروني فارغاً")]
        [EmailAddress(ErrorMessage = "يجب أن يكون البريد الإلكتروني بصيغة صحيحة")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "لا يمكن أن يكون رقم الهاتف فارغاً")]
        [RegularExpression(@"^(01[0125][0-9]{8}|1[0125][0-9]{8})$", ErrorMessage = "يرجى إدخال رقم هاتف مصري صحيح (مثال: 1002211457)")]
        public string Phone { get; set; } = string.Empty;
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "يجب أن يكون الوصف بين 10 و 1000 حرف")]
        public string? Description { get; set; }
        [RegularExpression(@"^(01[0125][0-9]{8}|1[0125][0-9]{8})$", ErrorMessage = "يرجى إدخال رقم واتساب مصري صحيح (مثال: 1002211457)")]
        public string? Whatsapp { get; set; }
        public string? City { get; set; }
        public string? Governorate { get; set; }
        public string? PostalCode { get; set; }

        [Required(ErrorMessage = "لا يمكن أن تكون كلمة المرور فارغة")]
        [MinLength(5, ErrorMessage = "يجب أن تتكون كلمة المرور من 5 أحرف على الأقل")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "لا يمكن أن يكون تأكيد كلمة المرور فارغاً")]
        [Compare(nameof(Password), ErrorMessage = "كلمة المرور وتأكيد كلمة المرور غير متطابقين")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}