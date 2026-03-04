using ApartmentManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace ApartmentManagementSystem.Infrastructure.Persistence.EntityConfigurations
{
    public class FlatConfig : IEntityTypeConfiguration<Flat>
    {
        public void Configure(EntityTypeBuilder<Flat> builder)
        {
            builder.HasKey(f => f.Id);

            builder.Property(f => f.FlatNumber)
                .IsRequired()
                .HasMaxLength(10);

            builder.HasIndex(f => new { f.ApartmentId, f.FlatNumber })
                .IsUnique();
            builder
   .HasOne(f => f.Apartment)
   .WithMany(a => a.Flats)
   .HasForeignKey(f => f.ApartmentId)
   .OnDelete(DeleteBehavior.Restrict);
     builder.HasOne(f => f.OwnerUser)
     .WithMany()
     .HasForeignKey(f => f.OwnerUserId)
      .OnDelete(DeleteBehavior.SetNull);
        }
    }
}