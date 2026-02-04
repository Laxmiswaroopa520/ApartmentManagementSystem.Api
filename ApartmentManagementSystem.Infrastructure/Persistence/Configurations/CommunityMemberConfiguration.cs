using ApartmentManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApartmentManagementSystem.Infrastructure.Persistence.Configurations;

public class CommunityMemberConfiguration : IEntityTypeConfiguration<CommunityMember>
{
    public void Configure(EntityTypeBuilder<CommunityMember> builder)
    {
        builder.HasKey(cm => cm.Id);

        builder.Property(cm => cm.CommunityRole)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(cm => cm.AssignedAt)
            .IsRequired();

        builder.Property(cm => cm.AssignedBy)
            .IsRequired();

        builder.Property(cm => cm.IsActive)
            .IsRequired();

        // Relationships
        builder.HasOne(cm => cm.User)
            .WithMany()
            .HasForeignKey(cm => cm.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(cm => cm.Apartment)
            .WithMany(a => a.CommunityMembers)
            .HasForeignKey(cm => cm.ApartmentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(cm => new { cm.ApartmentId, cm.CommunityRole, cm.IsActive });
        builder.HasIndex(cm => cm.UserId);
    }
}