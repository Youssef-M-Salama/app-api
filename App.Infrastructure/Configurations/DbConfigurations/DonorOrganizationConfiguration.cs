using App.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using App.Core.Domain.Enums;

namespace App.Infrastructure.Configurations.DbConfigurations
{
    public class DonorOrganizationConfiguration : IEntityTypeConfiguration<DonorOrganization>
    {
        public void Configure(EntityTypeBuilder<DonorOrganization> builder)
        {
            // Table name
            builder.ToTable("DonorOrganizations");

            // Primary Key
            builder.HasKey(d => d.DonorOrganizationId);

            // Properties
            builder.Property(d => d.DonorOrganizationId)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(d => d.DonorOrganizationName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(d => d.VerificationState)
                .HasConversion(new EnumToStringConverter<VerificationState>())
                .IsRequired()
                .HasDefaultValue(VerificationState.Pending)
                .HasComment("Admin verification status");

            builder.Property(d => d.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(d => d.DonorOrganizationDescription)
                    .HasMaxLength(1000);

            // Optional Verification Data
            builder.Property(d => d.CommercialRegistrationNumber).HasMaxLength(50);
            builder.Property(d => d.TaxNumber).HasMaxLength(50);
            builder.Property(d => d.BusinessLicenseNumber).HasMaxLength(50);
            builder.Property(d => d.HeadquartersAddress).HasMaxLength(500);

            builder.Property(d => d.CommercialRegisterUrl).HasMaxLength(500);
            builder.Property(d => d.TaxCardUrl).HasMaxLength(500);
            builder.Property(d => d.BusinessLicenseUrl).HasMaxLength(500);
            builder.Property(d => d.CivilProtectionApprovalUrl).HasMaxLength(500);
            builder.Property(d => d.EnvironmentalApprovalUrl).HasMaxLength(500);
            builder.Property(d => d.OwnershipContractUrl).HasMaxLength(500);

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

            builder.HasIndex(d => d.DonorOrganizationName)
                .HasDatabaseName("IX_DonorOrganizations_DonorOrganizationName");

            builder.HasIndex(d => d.VerificationState)
                .HasDatabaseName("IX_DonorOrganizations_VerificationState");

            builder.HasIndex(d => d.IsActive)
                .HasDatabaseName("IX_DonorOrganizations_IsActive");
        }
    }
}