using ApartmentManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace ApartmentManagementSystem.Infrastructure.Persistence.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasKey(r => r.Id);

            //  No explicit column types - EF will handle per provider
            builder.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(100); // Specific length instead of max

            builder.Property(r => r.Description)
                .HasMaxLength(500); // Specific length instead of max

            builder.Property(r => r.CreatedAt)
                .IsRequired();

            // Optional: Add index for performance
            builder.HasIndex(r => r.Name);
        }
    }
}