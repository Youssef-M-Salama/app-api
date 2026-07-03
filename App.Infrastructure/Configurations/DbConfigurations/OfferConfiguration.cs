using App.Core.Domain.Entities;
using App.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace App.Infrastructure.Configurations.DbConfigurations
{
    public class OfferConfiguration : IEntityTypeConfiguration<Offer>
    {
        public void Configure(EntityTypeBuilder<Offer> builder)
        {
            // Table name
            builder.ToTable("Offers");

            // Primary Key
            builder.HasKey(o => o.OfferId);

            // Properties
            builder.Property(o => o.OfferId)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(o => o.DonorOrganizationId)
                .IsRequired();

            builder.Property(o => o.AdminId)
                .IsRequired(false)
                .HasComment("Assigned when admin approves or rejects the offer");

            builder.Property(o => o.Category)
                .HasConversion(new EnumToStringConverter<ProductCategory>())
                .IsRequired();

            builder.Property(o => o.ProductName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(o => o.Quantity)
                .IsRequired()
                .HasPrecision(18, 2)
                .HasComment("Quantity available");

            builder.Property(o => o.Unit)
                .HasConversion(new EnumToStringConverter<MeasurementUnit>())
                .IsRequired()
                .HasDefaultValue(MeasurementUnit.Piece);

            builder.Property(o => o.ProductImage)
                .HasMaxLength(500)
                .IsUnicode(false);

            builder.Property(o => o.ExpiryDate)
                .IsRequired()
                .HasComment("When offer expires");

            builder.Property(o => o.Status)
                .HasConversion(new EnumToStringConverter<OfferStatus>())

                .IsRequired()
                .HasDefaultValue(OfferStatus.Pending);

            builder.Property(o => o.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(o => o.UpdatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationships
            builder.HasOne(o => o.DonorOrganization)
                .WithMany(d => d.Offers)
                .HasForeignKey(o => o.DonorOrganizationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(o => o.OfferApplications)
                .WithOne(oa => oa.Offer)
                .HasForeignKey(oa => oa.OfferId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(o => o.DonorOrganizationId)
                .HasDatabaseName("IX_Offers_DonorOrganizationId");

            builder.HasIndex(o => o.Status)
                .HasDatabaseName("IX_Offers_Status");

            builder.HasIndex(o => o.Category)
                .HasDatabaseName("IX_Offers_Category");

            builder.HasIndex(o => o.ExpiryDate)
                .HasDatabaseName("IX_Offers_ExpiryDate");

            builder.HasIndex(o => new { o.Status, o.Category })
                .HasDatabaseName("IX_Offers_Status_Category");

            builder.HasIndex(o => o.CreatedAt)
                .HasDatabaseName("IX_Offers_CreatedAt");
        }
    }
}