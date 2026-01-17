using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApartmentManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApartmentManagementSystem.Infrastructure.Persistence.EntityConfigurations
{    public class ApartmentConfig : IEntityTypeConfiguration<Apartment>
    {
        public void Configure(EntityTypeBuilder<Apartment> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.Address)
                .IsRequired()
                .HasMaxLength(200);

            builder.HasMany(a => a.Flats)
                .WithOne(f => f.Apartment)
                .HasForeignKey(f => f.ApartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
