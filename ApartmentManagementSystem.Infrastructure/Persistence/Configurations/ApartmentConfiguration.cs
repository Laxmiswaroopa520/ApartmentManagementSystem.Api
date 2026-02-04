using ApartmentManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApartmentManagementSystem.Infrastructure.Persistence.Configurations;

public class ApartmentConfiguration : IEntityTypeConfiguration<Apartment>
{
    public void Configure(EntityTypeBuilder<Apartment> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.Address)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(a => a.City)
            .HasMaxLength(100);

        builder.Property(a => a.State)
            .HasMaxLength(100);

        builder.Property(a => a.PinCode)
            .HasMaxLength(10);

        builder.Property(a => a.TotalFloors)
            .IsRequired();

        builder.Property(a => a.FlatsPerFloor)
            .IsRequired();

        builder.Property(a => a.TotalFlats)
            .IsRequired();

        builder.Property(a => a.IsActive)
            .IsRequired();

        builder.Property(a => a.Status)
            .IsRequired();

        builder.Property(a => a.CreatedAt)
            .IsRequired();

        builder.Property(a => a.CreatedBy)
            .IsRequired();

        builder.Property(a => a.UpdatedAt);

        builder.Property(a => a.UpdatedBy);

        // Indexes
        builder.HasIndex(a => a.Name);
        builder.HasIndex(a => a.IsActive);
        builder.HasIndex(a => a.Status);
    }
}