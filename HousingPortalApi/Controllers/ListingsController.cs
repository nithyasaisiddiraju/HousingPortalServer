using HousingPortalApi.Data;
using HousingPortalApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HousingPortalApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ListingsController : Controller
    {
       private readonly HousingPortalDbContext _housingPortalDbContext;
        public ListingsController(HousingPortalDbContext housingPortalDbContext)
        {
            _housingPortalDbContext = housingPortalDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllListings()
        {
            var listings = await _housingPortalDbContext.Listings.ToListAsync();

            return Ok(listings);
        }

        [HttpPost]
        public async Task<IActionResult> AddListing([FromBody] Listing listingRequest)
        {
            listingRequest.Id = Guid.NewGuid();

            await _housingPortalDbContext.Listings.AddAsync(listingRequest);
            await _housingPortalDbContext.SaveChangesAsync();

            return Ok(listingRequest);
            
        }
    }
}
