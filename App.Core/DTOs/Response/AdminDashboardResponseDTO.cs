namespace App.Core.DTOs.Response
{
    public class AdminDashboardResponseDTO
    {
        public int PendingVerifications { get; set; }
        public int PendingCharityNeeds { get; set; }
        public int PendingOffers { get; set; }
        public int TotalUsers { get; set; }
        public int ActiveCharityNeeds { get; set; }
        public int ActiveOffers { get; set; }
    }
}
