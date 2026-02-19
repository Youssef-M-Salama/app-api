using App.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Infrastructure.Configurations.DbConfigurations
{
    internal class OfferApplicationConfiguration : IEntityTypeConfiguration<OfferApplication>
    {
        public void Configure(EntityTypeBuilder<OfferApplication> builder)
        {
            // Table name
            builder.ToTable("OfferApplications");

            // Primary Key
            builder.HasKey(oa => oa.OfferApplicationId);

            // Properties
            builder.Property(oa => oa.OfferApplicationId)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(oa => oa.OfferId)
                .IsRequired();

            builder.Property(oa => oa.CharityId)
                .IsRequired();

            builder.Property(oa => oa.Status)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue("pending")
                .HasComment("pending, accepted, rejected");

            builder.Property(oa => oa.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(oa => oa.UpdatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationships
            builder.HasOne(oa => oa.Charity)
                .WithMany(c => c.OfferApplications)
                .HasForeignKey(oa => oa.CharityId)
                .OnDelete(DeleteBehavior.Restrict);  

            builder.HasOne(oa => oa.Offer)
                .WithMany(o => o.OfferApplications)
                .HasForeignKey(oa => oa.OfferId)
                .OnDelete(DeleteBehavior.Cascade);  

            // Indexes
            builder.HasIndex(oa => oa.CharityId)
                .HasDatabaseName("IX_OfferApplications_CharityId");

            builder.HasIndex(oa => oa.OfferId)
                .HasDatabaseName("IX_OfferApplications_OfferId");

            builder.HasIndex(oa => oa.Status)
                .HasDatabaseName("IX_OfferApplications_Status");

            builder.HasIndex(oa => new { oa.OfferId, oa.Status })
                .HasDatabaseName("IX_OfferApplications_OfferId_Status");

            builder.HasIndex(oa => new { oa.CharityId, oa.Status })
                .HasDatabaseName("IX_OfferApplications_CharityId_Status");

            // Unique constraint - prevent duplicate applications
            builder.HasIndex(oa => new { oa.CharityId, oa.OfferId })
                .IsUnique()
                .HasDatabaseName("IX_OfferApplications_Unique_CharityOffer");

            builder.HasIndex(oa => oa.CreatedAt)
                .HasDatabaseName("IX_OfferApplications_CreatedAt");
        }
    }
}