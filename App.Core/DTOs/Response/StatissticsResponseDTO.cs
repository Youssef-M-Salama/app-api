namespace App.Core.DTOs.Response
{
    /// <summary>
    /// Platform-wide statistics returned on the public landing page.
    /// GET /api/v1/public/statistics
    /// </summary>
    public class StatisticsResponseDto
    {
        /// <summary>Total fulfilled CharityNeeds and Offers combined.</summary>
        public int TotalDonations { get; set; }

        /// <summary>Total verified and active charities on the platform.</summary>
        public int TotalCharities { get; set; }

        /// <summary>Total verified and active donor organizations on the platform.</summary>
        public int TotalDonors { get; set; }

        /// <summary>CharityNeeds with status <c>Approved</c>.</summary>
        public int ActiveCharityNeeds { get; set; }

        /// <summary>Offers with status <c>Approved</c>.</summary>
        public int ActiveOffers { get; set; }

        /// <summary>Total quantity across all fulfilled CharityNeeds and Offers.</summary>
        public int TotalItemsDonated { get; set; }
    }
}