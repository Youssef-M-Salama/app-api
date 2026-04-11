using App.Core.Enums;

namespace App.Core.DTOs.Response
{
    /// <summary>
    /// Represents a single offer item in the public offers list.
    /// </summary>
    public class OfferResponseDTO
    {
        /// <summary>Unique identifier of the offer.</summary>
        public Guid OfferId { get; set; }

        /// <summary>Name of the donor organization that posted the offer.</summary>
        public string DonorOrganizationName { get; set; } = string.Empty;

        /// <summary>Name of the offered product.</summary>
        public string ProductName { get; set; } = string.Empty;

        /// <summary>Category of the offer (food, clothing, medical, education).</summary>
        public ProductCategory Category { get; set; }

        /// <summary>City where the donor organization is located.</summary>
        public string? City { get; set; }

        /// <summary>Governorate where the donor organization is located.</summary>
        public string? Governorate { get; set; }

        /// <summary>Quantity of the offered product.</summary>
        public int Quantity { get; set; }

        /// <summary>URL of the product image.</summary>
        public string? ProductImage { get; set; }

        /// <summary>Expiry date of the offer.</summary>
        public DateTime ExpiryDate { get; set; }

        /// <summary>Current status of the offer.</summary>
        /// <summary>0: Pending, 1: Approved, 2: Rejected, 3: Expired, 4: Fulfilled</summary>
        public OfferStatus Status { get; set; }

        /// <summary>UTC date and time when the offer was created.</summary>
        public DateTime CreatedAt { get; set; }
    }
}