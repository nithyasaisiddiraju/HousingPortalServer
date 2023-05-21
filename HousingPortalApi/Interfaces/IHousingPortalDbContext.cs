using HousingPortalApi.Models;
using Microsoft.EntityFrameworkCore;

namespace HousingPortalApi.Interfaces
{
    public interface IHousingPortalDbContext
    {
        DbSet<Listing> Listings { get; set; }
        DbSet<Student> Students { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
