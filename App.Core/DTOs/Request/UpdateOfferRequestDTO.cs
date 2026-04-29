using App.Core.Enums;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace App.Core.DTOs.Request
{
    /// <summary>
    /// Request body for updating an offer.
    /// All fields are optional — only non-null values are applied.
    /// Must be submitted as multipart/form-data to support optional image upload.
    /// </summary>
    public class UpdateOfferRequestDTO
    {
        /// <summary>0: Food, 1: Clothing, 2: Medical, 3: Education, 4: Other</summary>
        public ProductCategory? Category { get; set; }

        [StringLength(200, ErrorMessage = "يجب ألا يتجاوز اسم المنتج 200 حرف")]
        public string? ProductName { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "يجب أن تكون الكمية 1 على الأقل")]
        public int? Quantity { get; set; }

        public DateTime? ExpiryDate { get; set; }

        /// <summary>
        /// Optional new product image. Replaces the existing image if provided.
        /// Allowed formats: .jpg, .jpeg, .png, .webp. Max 2MB.
        /// </summary>
        public IFormFile? ProductImage { get; set; }
    }
}