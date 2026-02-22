using App.Core.Entities;
using App.Core.IdentityEntities;
using App.Infrastructure.Configurations.DbConfigurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace App.Infrastructure.DbContext
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public virtual DbSet<Charity> Charities { get; set; }
        public virtual DbSet<DonorOrganization> DonorOrganizations { get; set; }
        public virtual DbSet<CharityNeed> CharityNeeds { get; set; }
        public virtual DbSet<Offer> Offers { get; set; }
        public virtual DbSet<NeedApplication> NeedApplications { get; set; }
        public virtual DbSet<OfferApplication> OfferApplications { get; set; }
        public virtual DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public virtual DbSet<ApplicationRole> ApplicationRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new ApplicationUserConfiguration());
            modelBuilder.ApplyConfiguration(new CharityConfiguration());
            modelBuilder.ApplyConfiguration(new CharityNeedConfiguration());
            modelBuilder.ApplyConfiguration(new DonorOrganizationConfiguration());
            modelBuilder.ApplyConfiguration(new NeedApplicationConfiguration());
            modelBuilder.ApplyConfiguration(new OfferApplicationConfiguration());
            modelBuilder.ApplyConfiguration(new OfferConfiguration());
        }
    }
}
