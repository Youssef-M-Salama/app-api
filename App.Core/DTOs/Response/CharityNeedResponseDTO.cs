using App.Core.Enums;

namespace App.Core.DTOs.Response
{
    /// <summary>
    /// Represents a single charity need item in the public requests list.
    /// </summary>
    public class CharityNeedResponseDTO
    {
        public Guid CharityNeedId { get; set; }
        public string CharityName { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        /// <summary>0: Food, 1: Clothing, 2: Medical, 3: Education, 4: Other</summary>
        public ProductCategory Category { get; set; }
        public string? City { get; set; }
        public string? Governorate { get; set; }
        public int Quantity { get; set; }
        /// <summary>0: Urgent, 1: High, 2: Normal, 3: Low</summary>
        public CharityNeedPriority Priority { get; set; }
        /// <summary>0: Pending, 1: Approved, 2: Rejected, 3: Fulfilled</summary>
        public CharityNeedStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? ProductImage { get; set; }
    }
}