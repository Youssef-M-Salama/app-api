namespace App.Core.DTO.Response
{
    /// <summary>
    /// Represents a single charity need item in the public requests list.
    /// </summary>
    public class CharityNeedResponseDto
    {
        /// <summary>
        /// Unique identifier of the charity need.
        /// </summary>
        public Guid CharityNeedId { get; set; }

        /// <summary>
        /// Name of the charity that posted the request.
        /// </summary>
        public string CharityName { get; set; } = string.Empty;

        /// <summary>
        /// Name of the requested product.
        /// </summary>
        public string ProductName { get; set; } = string.Empty;

        /// <summary>
        /// Category of the request (food, clothing, medical, education).
        /// </summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// City where the charity is located.
        /// </summary>
        public string? City { get; set; }

        /// <summary>
        /// Governorate where the charity is located.
        /// </summary>
        public string? Governorate { get; set; }

        /// <summary>
        /// Quantity of the requested product.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Priority level of the request (urgent, high, normal, low).
        /// </summary>
        public string Priority { get; set; } = string.Empty;

        /// <summary>
        /// Current status of the request.
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// UTC date and time when the request was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}