using App.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Infrastructure.Configurations.DbConfigurations
{
    internal class CharityNeedConfiguration : IEntityTypeConfiguration<CharityNeed>
    {
        public void Configure(EntityTypeBuilder<CharityNeed> builder)
        {
            // Table name
            builder.ToTable("CharityNeeds");

            // Primary Key
            builder.HasKey(cn => cn.CharityNeedId);

            // Properties
            builder.Property(cn => cn.CharityNeedId)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(cn => cn.CharityId)
                .IsRequired();

            builder.Property(cn => cn.AdminId)
                .IsRequired(false)
                .HasComment("Assigned when admin approves request");

            builder.Property(cn => cn.Category)
                .IsRequired()
                .HasMaxLength(100)
                .HasComment("food, clothing, medical, education, etc");

            builder.Property(cn => cn.ProductName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(cn => cn.Quantity)
                .IsRequired()
                .HasComment("Quantity needed");

            builder.Property(cn => cn.Priority)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue("normal")
                .HasComment("urgent, high, normal, low");

            builder.Property(cn => cn.Status)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue("pending")
                .HasComment("pending, approved, rejected, fulfilled");

            builder.Property(cn => cn.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(cn => cn.UpdatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationships
            builder.HasOne(cn => cn.Charity)
                .WithMany(c => c.CharityNeeds)
                .HasForeignKey(cn => cn.CharityId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(cn => cn.NeedApplications)
                .WithOne(na => na.CharityNeed)
                .HasForeignKey(na => na.CharityNeedId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(cn => cn.CharityId)
                .HasDatabaseName("IX_CharityNeeds_CharityId");

            builder.HasIndex(cn => cn.Status)
                .HasDatabaseName("IX_CharityNeeds_Status");

            builder.HasIndex(cn => cn.Category)
                .HasDatabaseName("IX_CharityNeeds_Category");

            builder.HasIndex(cn => cn.Priority)
                .HasDatabaseName("IX_CharityNeeds_Priority");

            builder.HasIndex(cn => new { cn.Status, cn.Category })
                .HasDatabaseName("IX_CharityNeeds_Status_Category");

            builder.HasIndex(cn => cn.CreatedAt)
                .HasDatabaseName("IX_CharityNeeds_CreatedAt");
        }
    }
}