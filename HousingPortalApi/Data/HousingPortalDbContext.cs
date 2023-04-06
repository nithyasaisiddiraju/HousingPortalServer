using HousingPortalApi.Models;
using Microsoft.EntityFrameworkCore;

namespace HousingPortalApi.Data
{
    public class HousingPortalDbContext : DbContext
    {
        public HousingPortalDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Listing> Listings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Listing>(entity =>
            {
                entity.Property(e => e.Price)
                    .HasColumnType("decimal(18,2)");
            });
        }
    }
}