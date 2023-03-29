using Microsoft.EntityFrameworkCore;

namespace HousingPortalApi.Models
{
    public class OffCampusHousingContext : DbContext
    {
        public OffCampusHousingContext(DbContextOptions<OffCampusHousingContext> options)
            : base(options)
        {
        }

        public DbSet<OffCampusListing> OffCampusListings { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OffCampusListing>()
                .Property(listing => listing.Price)
                .HasColumnType("decimal(18, 2)")
                .HasPrecision(18, 2);
        }
    }
}
