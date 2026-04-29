using App.Core.Enums;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace App.Core.DTOs.Request
{
    /// <summary>
    /// Request body for creating a new offer.
    /// Must be submitted as multipart/form-data to support optional image upload.
    /// </summary>
    public class CreateOfferRequestDTO
    {
        /// <summary>0: Food, 1: Clothing, 2: Medical, 3: Education, 4: Other</summary>
        public ProductCategory Category { get; set; }

        [Required(ErrorMessage = "اسم المنتج مطلوب")]
        [StringLength(200, ErrorMessage = "يجب ألا يتجاوز اسم المنتج 200 حرف")]
        public string ProductName { get; set; } = string.Empty;

        [Required(ErrorMessage = "الكمية مطلوبة")]
        [Range(1, int.MaxValue, ErrorMessage = "يجب أن تكون الكمية 1 على الأقل")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "تاريخ الانتهاء مطلوب")]
        public DateTime ExpiryDate { get; set; }

        /// <summary>Optional product image. Allowed formats: .jpg, .jpeg, .png, .webp. Max 2MB.</summary>
        public IFormFile? ProductImage { get; set; }
    }
}