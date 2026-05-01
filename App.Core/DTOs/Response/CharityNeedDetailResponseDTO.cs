using App.Core.Enums;
namespace App.Core.DTOs.Response
{
    /// <summary>
    /// Full detail of a charity need — returned to the owning charity.
    /// Includes ProductImage and all status/timestamp fields.
    /// </summary>
    public class CharityNeedDetailResponseDTO
    {
        public Guid CharityNeedId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        /// <summary>0: Food, 1: Clothing, 2: Medical, 3: Education, 4: Other</summary>
        public ProductCategory Category { get; set; } 
        public int Quantity { get; set; }
        public string? ProductImage { get; set; }
        /// <summary>0: Urgent, 1: High, 2: Normal, 3: Low</summary>
        public CharityNeedPriority Priority { get; set; }
        /// <summary>0: Pending, 1: Approved, 2: Rejected, 3: Fulfilled</summary>
        public CharityNeedStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? Description { get; set; }
        public string? CharityDescription { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Whatsapp { get; set; }
    }
}