using ApartmentManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApartmentManagementSystem.Infrastructure.Persistence.Configurations;

public class UserFlatMappingConfiguration : IEntityTypeConfiguration<UserFlatMapping>
{
    public void Configure(EntityTypeBuilder<UserFlatMapping> builder)
    {
        builder.HasKey(ufm => ufm.Id);

        builder.Property(ufm => ufm.RelationshipType)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(ufm => ufm.FromDate)
            .IsRequired();

        builder.Property(ufm => ufm.ToDate);

        builder.Property(ufm => ufm.IsActive)
            .IsRequired();

        builder.Property(ufm => ufm.CreatedAt)
            .IsRequired();

        // Relationships
        builder.HasOne(ufm => ufm.User)
            .WithMany(u => u.UserFlatMappings)
            .HasForeignKey(ufm => ufm.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ufm => ufm.Flat)
            .WithMany(f => f.UserFlatMappings)
            .HasForeignKey(ufm => ufm.FlatId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(ufm => new { ufm.UserId, ufm.FlatId, ufm.IsActive });
    }
}