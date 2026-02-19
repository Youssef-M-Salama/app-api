using App.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Infrastructure.Configurations.DbConfigurations
{
    internal class DonorOrganizationConfiguration : IEntityTypeConfiguration<DonorOrganization>
    {
        public void Configure(EntityTypeBuilder<DonorOrganization> builder)
        {
            // Table name
            builder.ToTable("DonorOrganizations");

            // Primary Key
            builder.HasKey(d => d.DonorId);

            // Properties
            builder.Property(d => d.DonorId)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(d => d.DonorName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(d => d.DonorOrganizationImage)
                .HasMaxLength(500)
                .IsUnicode(false);

            builder.Property(d => d.IsVerified)
                .IsRequired()
                .HasDefaultValue(false)
                .HasComment("Admin verification status");

            builder.Property(d => d.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(d => d.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(d => d.UpdatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(d => d.UserId)
                .IsRequired();

            // Relationships
            builder.HasOne(d => d.ApplicationUser)
                .WithOne(u => u.DonorOrganization)
                .HasForeignKey<DonorOrganization>(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(d => d.Offers)
                .WithOne(o => o.DonorOrganization)
                .HasForeignKey(o => o.DonorOrganizationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(d => d.NeedApplications)
                .WithOne(na => na.DonorOrganization)
                .HasForeignKey(na => na.DonorOrganizationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(d => d.UserId)
                .IsUnique()
                .HasDatabaseName("IX_DonorOrganizations_UserId");

            builder.HasIndex(d => d.DonorName)
                .HasDatabaseName("IX_DonorOrganizations_DonorName");

            builder.HasIndex(d => d.IsVerified)
                .HasDatabaseName("IX_DonorOrganizations_IsVerified");

            builder.HasIndex(d => d.IsActive)
                .HasDatabaseName("IX_DonorOrganizations_IsActive");
        }
    }
}