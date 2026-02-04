using ApartmentManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApartmentManagementSystem.Infrastructure.Persistence.Configurations;

public class UserOtpConfiguration : IEntityTypeConfiguration<UserOtp>
{
    public void Configure(EntityTypeBuilder<UserOtp> builder)
    {
        builder.HasKey(uo => uo.Id);

        builder.Property(uo => uo.PhoneNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(uo => uo.OtpCode)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(uo => uo.ExpiresAt)
            .IsRequired();

        builder.Property(uo => uo.IsUsed)
            .IsRequired();

        builder.Property(uo => uo.CreatedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(uo => uo.PhoneNumber);
        builder.HasIndex(uo => new { uo.PhoneNumber, uo.OtpCode });
        builder.HasIndex(uo => uo.ExpiresAt);
    }
}