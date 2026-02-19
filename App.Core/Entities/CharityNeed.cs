namespace App.Core.Entities
{
    public class CharityNeed
    {
        public Guid CharityNeedId { get; set; }
        public Guid CharityId { get; set; }
        public Guid? AdminId { get; set; }  //  Nullable - assigned after approval
        public string Category { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }  
        public string Priority { get; set; }  // 'urgent', 'high', 'normal', 'low'
        public string Status { get; set; }  
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation Properties
        public Charity Charity { get; set; }
        public ICollection<NeedApplication> NeedApplications { get; set; }
    }
}