using ApartmentManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApartmentManagementSystem.Infrastructure.Persistence.Configurations;

public class UserInviteConfiguration : IEntityTypeConfiguration<UserInvite>
{
    public void Configure(EntityTypeBuilder<UserInvite> builder)
    {
        builder.HasKey(ui => ui.Id);

        builder.Property(ui => ui.FullName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(ui => ui.PrimaryPhone)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(ui => ui.ResidentType)
            .IsRequired();

        builder.Property(ui => ui.InviteStatus)
            .IsRequired()
            .HasMaxLength(50); // ✅ Specific length instead of max

        builder.Property(ui => ui.CreatedAt)
            .IsRequired();

        builder.Property(ui => ui.CreatedByUserId)
            .IsRequired();

        // Relationships
        builder.HasOne(ui => ui.Role)
            .WithMany()
            .HasForeignKey(ui => ui.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(ui => ui.PrimaryPhone);
    }
}