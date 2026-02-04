using ApartmentManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//for testing part..
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