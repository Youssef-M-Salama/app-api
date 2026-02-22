using App.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Infrastructure.Configurations.DbConfigurations
{
    internal class NeedApplicationConfiguration : IEntityTypeConfiguration<NeedApplication>
    {
        public void Configure(EntityTypeBuilder<NeedApplication> builder)
        {
            // Table name
            builder.ToTable("NeedApplications");

            // Primary Key
            builder.HasKey(na => na.NeedApplicationId);

            // Properties
            builder.Property(na => na.NeedApplicationId)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(na => na.DonorOrganizationId)
                .IsRequired();

            builder.Property(na => na.CharityNeedId)
                .IsRequired();

            builder.Property(na => na.Status)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue("pending")
                .HasComment("pending, accepted, rejected");

            builder.Property(na => na.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(na => na.UpdatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationships
            builder.HasOne(na => na.DonorOrganization)
                .WithMany(d => d.NeedApplications)
                .HasForeignKey(na => na.DonorOrganizationId)
                .OnDelete(DeleteBehavior.Restrict); 

            builder.HasOne(na => na.CharityNeed)
                .WithMany(cn => cn.NeedApplications)
                .HasForeignKey(na => na.CharityNeedId)
                .OnDelete(DeleteBehavior.Cascade);  

            // Indexes
            builder.HasIndex(na => na.DonorOrganizationId)
                .HasDatabaseName("IX_NeedApplications_DonorOrganizationId");

            builder.HasIndex(na => na.CharityNeedId)
                .HasDatabaseName("IX_NeedApplications_CharityNeedId");

            builder.HasIndex(na => na.Status)
                .HasDatabaseName("IX_NeedApplications_Status");

            builder.HasIndex(na => new { na.CharityNeedId, na.Status })
                .HasDatabaseName("IX_NeedApplications_CharityNeedId_Status");

            builder.HasIndex(na => new { na.DonorOrganizationId, na.Status })
                .HasDatabaseName("IX_NeedApplications_DonorOrganizationId_Status");

            // Unique constraint - prevent duplicate applications
            builder.HasIndex(na => new { na.DonorOrganizationId, na.CharityNeedId })
                .IsUnique()
                .HasDatabaseName("IX_NeedApplications_Unique_DonorNeed");

            builder.HasIndex(na => na.CreatedAt)
                .HasDatabaseName("IX_NeedApplications_CreatedAt");
        }
    }
}