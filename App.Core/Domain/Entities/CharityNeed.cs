using App.Core.Enums;

namespace App.Core.Domain.Entities
{
    public class CharityNeed
    {
        public Guid CharityNeedId { get; set; }
        public Guid CharityId { get; set; }
        public Guid? AdminId { get; set; }
        public string Category { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public string? ProductImage { get; set; }
        public CharityNeedPriority Priority { get; set; }
        public CharityNeedStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation Properties
        public Charity Charity { get; set; }
        public ICollection<NeedApplication> NeedApplications { get; set; }
    }
}