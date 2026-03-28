using App.Core.Enums;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace App.Core.DTOs.Request
{
    /// <summary>
    /// Request body for creating a new charity need.
    /// Must be submitted as multipart/form-data to support optional image upload.
    /// </summary>
    public class CreateCharityNeedRequestDTO
    {
        [Required(ErrorMessage = "Category is required")]
        [StringLength(100, ErrorMessage = "Category must not exceed 100 characters")]
        public string Category { get; set; } = string.Empty;

        [Required(ErrorMessage = "Product name is required")]
        [StringLength(200, ErrorMessage = "Product name must not exceed 200 characters")]
        public string ProductName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Priority is required")]
        public CharityNeedPriority Priority { get; set; }

        /// <summary>Optional product image. Allowed formats: .jpg, .jpeg, .png, .webp. Max 2MB.</summary>
        public IFormFile? ProductImage { get; set; }
    }
}