using App.Core.Entities;
using App.Core.IdentityEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Infrastructure.Configurations.DbConfigurations
{
    internal class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            // Table name
            builder.ToTable("Users");

            // Properties
            builder.Property(u => u.Whatsapp)
                .HasMaxLength(20)
                .IsUnicode(false);

            builder.Property(u => u.City)
                .HasMaxLength(100);

            builder.Property(u => u.Governorate)
                .HasMaxLength(100);

            builder.Property(u => u.PostalCode)
                .HasMaxLength(20)
                .IsUnicode(false);

            builder.Property(u => u.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(u => u.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(u => u.UpdatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationships
            builder.HasOne(u => u.Charity)
                .WithOne(c => c.ApplicationUser)
                .HasForeignKey<Charity>(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(u => u.DonorOrganization)
                .WithOne(d => d.ApplicationUser)
                .HasForeignKey<DonorOrganization>(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(u => u.Email)
                .IsUnique()
                .HasDatabaseName("IX_Users_Email");

            builder.HasIndex(u => u.UserName)
                .IsUnique()
                .HasDatabaseName("IX_Users_UserName");

            builder.HasIndex(u => u.IsActive)
                .HasDatabaseName("IX_Users_IsActive");
        }
    }
}