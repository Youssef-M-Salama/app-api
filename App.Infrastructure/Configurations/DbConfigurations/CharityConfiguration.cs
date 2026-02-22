using App.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.Infrastructure.Configurations.DbConfigurations
{
    internal class CharityConfiguration : IEntityTypeConfiguration<Charity>
    {
        public void Configure(EntityTypeBuilder<Charity> builder)
        {
            // Table name
            builder.ToTable("Charities");

            // Primary Key
            builder.HasKey(c => c.CharityId);

            // Properties
            builder.Property(c => c.CharityId)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(c => c.CharityName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.CharityDescription)
                .HasMaxLength(1000);

            builder.Property(c => c.IsVerified)
                .IsRequired()
                .HasDefaultValue(false)
                .HasComment("Admin verification status");

            builder.Property(c => c.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(c => c.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(c => c.UpdatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(c => c.UserId)
                .IsRequired();

            // Relationships
            builder.HasOne(c => c.ApplicationUser)
                .WithOne(u => u.Charity)
                .HasForeignKey<Charity>(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.CharityNeeds)
                .WithOne(cn => cn.Charity)
                .HasForeignKey(cn => cn.CharityId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.OfferApplications)
                .WithOne(oa => oa.Charity)
                .HasForeignKey(oa => oa.CharityId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(c => c.UserId)
                .IsUnique()
                .HasDatabaseName("IX_Charities_UserId");

            builder.HasIndex(c => c.CharityName)
                .HasDatabaseName("IX_Charities_CharityName");

            builder.HasIndex(c => c.IsVerified)
                .HasDatabaseName("IX_Charities_IsVerified");

            builder.HasIndex(c => c.IsActive)
                .HasDatabaseName("IX_Charities_IsActive");
        }
    }
}