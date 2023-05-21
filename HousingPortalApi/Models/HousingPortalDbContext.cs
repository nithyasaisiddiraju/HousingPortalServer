﻿using HousingPortalApi.Interfaces;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace HousingPortalApi.Models
{
    public class HousingPortalDbContext : IdentityDbContext<HousingPortalUser>, IHousingPortalDbContext
    {
        public HousingPortalDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Listing> Listings { get; set; }
        public DbSet<Student> Students { get; set; }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Listing>(entity =>
            {
                entity.Property(e => e.price)
                    .HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.Property(e => e.name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.email)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.phone)
                    .HasMaxLength(20);

                entity.Property(e => e.major)
                    .HasMaxLength(100);

                entity.Property(e => e.graduationYear)
                    .HasMaxLength(4); ;
            });

            // One student can have multiple listings, and each listing is associated with a specific student
            modelBuilder.Entity<Listing>()
                .HasOne(listing => listing.student)
                .WithMany(student => student.listings)
                .HasForeignKey(listing => listing.studentId);
        }
    }
}
