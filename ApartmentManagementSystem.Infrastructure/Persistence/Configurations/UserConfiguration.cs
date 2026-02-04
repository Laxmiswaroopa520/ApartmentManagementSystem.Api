using ApartmentManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApartmentManagementSystem.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.FullName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(u => u.Username)
            .HasMaxLength(100);

        builder.Property(u => u.PasswordHash)
            .HasMaxLength(500);

        builder.Property(u => u.Email)
            .HasMaxLength(200);

        builder.Property(u => u.PrimaryPhone)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(u => u.SecondaryPhone)
            .HasMaxLength(20);

        builder.Property(u => u.Status)
            .IsRequired();

        builder.Property(u => u.ResidentType);

        builder.Property(u => u.IsActive)
            .IsRequired();

        builder.Property(u => u.IsOtpVerified)
            .IsRequired();

        builder.Property(u => u.IsRegistrationCompleted)
            .IsRequired();

        builder.Property(u => u.CreatedAt)
            .IsRequired();

        builder.Property(u => u.UpdatedAt);

        builder.Property(u => u.UpdatedBy);

        // Relationships
        builder.HasOne(u => u.Flat)
            .WithMany()
            .HasForeignKey(u => u.FlatId)
            .OnDelete(DeleteBehavior.NoAction);

        // Indexes
        builder.HasIndex(u => u.Username);
        builder.HasIndex(u => u.PrimaryPhone);
        builder.HasIndex(u => u.Email);
        builder.HasIndex(u => u.Status);
        builder.HasIndex(u => u.IsActive);
    }
}