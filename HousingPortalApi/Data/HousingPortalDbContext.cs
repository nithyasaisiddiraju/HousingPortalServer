using HousingPortalApi.Interfaces;
using HousingPortalApi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace HousingPortalApi.Data
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
                entity.Property(e => e.Price)
                    .HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Phone)
                    .HasMaxLength(20);

                entity.Property(e => e.Major)
                    .HasMaxLength(100);

                entity.Property(e => e.GraduationYear)
                    .HasMaxLength(4); ;
            });

            // One student can have multiple listings, and each listing is associated with a specific student
            modelBuilder.Entity<Listing>()
                .HasOne(listing => listing.Student)
                .WithMany(student => student.Listings)
                .HasForeignKey(listing => listing.StudentId);
        }
    }
}
