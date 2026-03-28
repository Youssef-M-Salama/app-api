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
        public string Category { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string? ProductImage { get; set; }
        public string Priority { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}