using App.Core.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace App.Core.Domain.IdentityEntities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string? Whatsapp { get; set; }
        public string? ImageUrl { get; set; }

        public string? City { get; set; }
        public string? Governorate { get; set; }
        public string? PostalCode { get; set; }
        public bool IsActive { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiration { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation Properties
        public DonorOrganization? DonorOrganization { get; set; }
        public Charity? Charity { get; set; }
    }
}