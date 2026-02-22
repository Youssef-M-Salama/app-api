namespace App.Core.Entities
{
    public class NeedApplication
    {
        public Guid NeedApplicationId { get; set; }
        public Guid DonorOrganizationId { get; set; }  
        public Guid CharityNeedId { get; set; }
        public string Status { get; set; }  
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation Properties
        public DonorOrganization DonorOrganization { get; set; }  
        public CharityNeed CharityNeed { get; set; }
    }
}