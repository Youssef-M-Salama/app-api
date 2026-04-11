using App.Core.Enums;
﻿namespace App.Core.DTOs.Response
{
    /// <summary>
    /// Full detail of a charity need — returned to the owning charity.
    /// Includes ProductImage and all status/timestamp fields.
    /// </summary>
    public class CharityNeedDetailResponseDTO
    {
        public Guid CharityNeedId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public ProductCategory Category { get; set; } 
        public int Quantity { get; set; }
        public string? ProductImage { get; set; }
        /// <summary>0: Urgent, 1: High, 2: Normal, 3: Low</summary>
        public CharityNeedPriority Priority { get; set; }
        /// <summary>0: Pending, 1: Approved, 2: Rejected, 3: Fulfilled</summary>
        public CharityNeedStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}