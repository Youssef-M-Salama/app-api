using App.Core.Enums;
﻿namespace App.Core.DTOs.Response
{
    /// <summary>
    /// Full detail of an offer — returned to the owning donor organization.
    /// Includes ProductImage and all status/timestamp fields.
    /// </summary>
    public class OfferDetailResponseDTO
    {
        public Guid OfferId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        /// <summary>0: Food, 1: Clothing, 2: Medical, 3: Education, 4: Other</summary>
        public ProductCategory Category { get; set; }
        public int Quantity { get; set; }
        public string? ProductImage { get; set; }
        public DateTime ExpiryDate { get; set; }
        /// <summary>0: Pending, 1: Approved, 2: Rejected, 3: Expired, 4: Fulfilled</summary>
        public OfferStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}