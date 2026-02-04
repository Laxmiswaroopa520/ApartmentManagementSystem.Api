using ApartmentManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApartmentManagementSystem.Infrastructure.Persistence.Configurations;

public class ApartmentManagerConfiguration : IEntityTypeConfiguration<ApartmentManager>
{
    public void Configure(EntityTypeBuilder<ApartmentManager> builder)
    {
        builder.HasKey(am => am.Id);

        builder.Property(am => am.AssignedAt)
            .IsRequired();

        builder.Property(am => am.RemovedAt)
            .IsRequired();

        builder.Property(am => am.AssignedBy)
            .IsRequired();

        builder.Property(am => am.IsActive)
            .IsRequired();

        // Relationships
        builder.HasOne(am => am.Apartment)
            .WithMany(a => a.Managers)
            .HasForeignKey(am => am.ApartmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(am => am.User)
            .WithMany()
            .HasForeignKey(am => am.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(am => new { am.ApartmentId, am.IsActive });
        builder.HasIndex(am => am.UserId);
    }
}