using App.Core.Enums;

namespace App.Core.DTO.Response
{
    /// <summary>
    /// Represents a single charity need item in the public requests list.
    /// </summary>
    public class CharityNeedResponseDto
    {
        public Guid CharityNeedId { get; set; }
        public string CharityName { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string? City { get; set; }
        public string? Governorate { get; set; }
        public int Quantity { get; set; }
        public string? Priority { get; set; }
        public string? Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}