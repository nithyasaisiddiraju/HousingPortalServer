using HousingPortalApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace HousingPortalApi.Interfaces
{
    public interface IHousingPortalDbContext
    {
        DbSet<Listing> Listings { get; }
        DbSet<Student> Students { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        int SaveChanges();
    }
}
