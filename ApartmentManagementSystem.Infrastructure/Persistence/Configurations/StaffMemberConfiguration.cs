using ApartmentManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApartmentManagementSystem.Infrastructure.Persistence.Configurations;

public class StaffMemberConfiguration : IEntityTypeConfiguration<StaffMember>
{
    public void Configure(EntityTypeBuilder<StaffMember> builder)
    {
        builder.HasKey(sm => sm.Id);

        builder.Property(sm => sm.FullName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(sm => sm.Phone)
            .IsRequired()
            .HasMaxLength(15);

        builder.Property(sm => sm.Email)
            .HasMaxLength(100);

        builder.Property(sm => sm.Address)
            .HasMaxLength(500);

        builder.Property(sm => sm.StaffType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(sm => sm.Specialization)
            .HasMaxLength(200);

        // ✅ SQLite-compatible: Use HasPrecision instead of HasColumnType("decimal(10,2)")
        builder.Property(sm => sm.HourlyRate)
            .HasPrecision(10, 2);

        builder.Property(sm => sm.IsActive)
            .IsRequired();

        builder.Property(sm => sm.JoinedOn)
            .IsRequired();

        builder.Property(sm => sm.CreatedBy)
            .IsRequired();

        builder.Property(sm => sm.CreatedAt)
            .IsRequired();

        builder.Property(sm => sm.UpdatedBy);

        builder.Property(sm => sm.UpdatedAt);

        // Relationships
        builder.HasOne(sm => sm.User)
            .WithMany()
            .HasForeignKey(sm => sm.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes
        builder.HasIndex(sm => sm.Phone);
        builder.HasIndex(sm => sm.StaffType);
        builder.HasIndex(sm => sm.IsActive);
    }
}