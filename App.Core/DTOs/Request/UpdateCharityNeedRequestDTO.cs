using App.Core.Enums;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace App.Core.DTOs.Request
{
    /// <summary>
    /// Request body for updating a pending charity need.
    /// All fields are optional — only non-null values are applied.
    /// Must be submitted as multipart/form-data to support optional image upload.
    /// </summary>
    public class UpdateCharityNeedRequestDTO
    {
        /// <summary>0: Food, 1: Clothing, 2: Medical, 3: Education, 4: Other</summary>
        public ProductCategory? Category { get; set; }

        [StringLength(200, ErrorMessage = "يجب ألا يتجاوز اسم المنتج 200 حرف")]
        public string? ProductName { get; set; }

        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "يجب أن تكون الكمية 0.01 على الأقل")]
        public decimal? Quantity { get; set; }

        public MeasurementUnit? Unit { get; set; }

        /// <summary>0: Urgent, 1: High, 2: Normal, 3: Low</summary>
        public CharityNeedPriority? Priority { get; set; }

        [StringLength(1000, ErrorMessage = "يجب ألا يتجاوز الوصف 1000 حرف")]
        public string? Description { get; set; }

        /// <summary>
        /// Optional new product image. Replaces the existing image if provided.
        /// Allowed formats: .jpg, .jpeg, .png, .webp. Max 2MB.
        /// </summary>
        public IFormFile? ProductImage { get; set; }
    }
}