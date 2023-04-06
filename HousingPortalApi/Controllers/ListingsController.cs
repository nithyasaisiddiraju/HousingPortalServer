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

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetListing([FromRoute] Guid id)
        {
            var listing = await _housingPortalDbContext.Listings.FirstOrDefaultAsync(x => x.Id == id);

           if (listing == null)
            {
                return NotFound();
            }
            return Ok(listing);
        }

        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> UpdateListing([FromRoute] Guid id, [FromBody] Listing updateListingRequest)
        {
            var listing = await _housingPortalDbContext.Listings.FindAsync(id);

            if (listing == null)
            {
                return NotFound();
            }
            listing.Title = updateListingRequest.Title;
            listing.Description = updateListingRequest.Description;
            listing.Address = updateListingRequest.Address;
            listing.Price = updateListingRequest.Price;
            listing.Contact = updateListingRequest.Contact;
            listing.Email = updateListingRequest.Email;

            await _housingPortalDbContext.SaveChangesAsync();

            return Ok(listing);

        }

        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> DeleteListing([FromRoute] Guid id)
        {
            var listing = await _housingPortalDbContext.Listings.FindAsync(id);

            if (listing == null)
            {
                return NotFound();
            }
            _housingPortalDbContext.Listings.Remove(listing);

            await _housingPortalDbContext.SaveChangesAsync();

            return Ok(listing);
        }

    }
}
