using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ApartmentManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApartmentManagementSystem.Infrastructure.Persistence.EntityConfigurations
{


    public class UserFlatMappingConfig : IEntityTypeConfiguration<UserFlatMapping>
    {
        public void Configure(EntityTypeBuilder<UserFlatMapping> builder)
        {
            builder.HasKey(m => m.Id);

            builder.Property(m => m.RelationshipType)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasOne(m => m.User)
                .WithMany(u => u.UserFlatMappings)
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            /*  builder.HasOne(m => m.Flat)
                  .WithMany(f => f.UserFlatMappings)
                  .HasForeignKey(m => m.FlatId)
                  .OnDelete(DeleteBehavior.Cascade);
            */
            builder.HasOne(m => m.Flat)
      .WithMany(f => f.UserFlatMappings)
      .HasForeignKey(m => m.FlatId)
      .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(m => new { m.UserId, m.FlatId, m.IsActive });
        }
    }
}