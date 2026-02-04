using ApartmentManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApartmentManagementSystem.Infrastructure.Persistence.Configurations;

public class FlatConfiguration : IEntityTypeConfiguration<Flat>
{
    public void Configure(EntityTypeBuilder<Flat> builder)
    {
        builder.HasKey(f => f.Id);

        builder.Property(f => f.FlatNumber)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(f => f.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(f => f.IsActive)
            .IsRequired();

        builder.Property(f => f.IsOccupied)
            .IsRequired();

        builder.Property(f => f.CreatedAt)
            .IsRequired();

        builder.Property(f => f.UpdatedAt);

        // Relationships
        builder.HasOne(f => f.Apartment)
            .WithMany(a => a.Flats)
            .HasForeignKey(f => f.ApartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(f => f.Floor)
            .WithMany(fl => fl.Flats)
            .HasForeignKey(f => f.FloorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(f => f.OwnerUser)
            .WithMany()
            .HasForeignKey(f => f.OwnerUserId)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes
        builder.HasIndex(f => new { f.ApartmentId, f.FlatNumber })
            .IsUnique();

        builder.HasIndex(f => f.OwnerUserId);
        builder.HasIndex(f => f.FloorId);
        builder.HasIndex(f => f.IsActive);
        builder.HasIndex(f => f.IsOccupied);
    }
}