using HousingPortalApi.Dtos;
using HousingPortalApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HousingPortalApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ListingsController : ControllerBase
    {
       private readonly HousingPortalDbContext _housingPortalDbContext;
       private readonly IWebHostEnvironment _hostingEnvironment;
        public ListingsController(HousingPortalDbContext housingPortalDbContext, IWebHostEnvironment hostingEnvironment)
        {
            _housingPortalDbContext = housingPortalDbContext;
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: api/Listings
        [HttpGet]
        public async Task<IActionResult> GetAllListings()
        {
            List<Listing> listings = await _housingPortalDbContext.Listings.ToListAsync();

            List<ListingDto> listingDtos = listings.Select(l => new ListingDto
            {
                Id = l.Id,
                Title = l.Title,
                Description = l.Description,
                Address = l.Address,
                Price = l.Price,
                City = l.City,
                State = l.State,
                Zip = l.Zip,
                Image = l.Image,
                StudentId = l.StudentId
            }).ToList();

            return Ok(listingDtos);
        }

        // POST: api/Listings
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddListing([FromBody] ListingDto listingDto)
        {
            string loggedInStudentId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(loggedInStudentId))
            {
                return BadRequest(new { message = "No student found with the given ID" });
            }

            Guid studentId;
            if (!Guid.TryParse(loggedInStudentId, out studentId))
            {
                return BadRequest(new { message = "Invalid student ID" });
            }
            Listing listing = new Listing
            {
                Id = Guid.NewGuid(),
                Title = listingDto.Title,
                Description = listingDto.Description,
                Address = listingDto.Address,
                Price = listingDto.Price,
                City = listingDto.City,
                State = listingDto.State,
                Zip = listingDto.Zip,
                Image = listingDto.Image,
                StudentId = listingDto.StudentId
            };

            if (Request.Form.Files.Count > 0)
            {
                var file = Request.Form.Files[0];
                var uploadPath = Path.Combine(_hostingEnvironment.WebRootPath, "images");
                var fileName = Path.GetFileName(file.FileName);
                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                listing.Image = "images/" + fileName;
            }

            Student student = await _housingPortalDbContext.Students.FindAsync(listingDto.StudentId);
            if (student == null)
            {
                return BadRequest(new { message = "No student found with the given ID" });
            }
            student.Listings.Add(listing);
            await _housingPortalDbContext.SaveChangesAsync();

            ListingDto createdListingDto = new ListingDto
            {
                Id = listing.Id,
                Title = listing.Title,
                Description = listing.Description,
                Address = listing.Address,
                Price = listing.Price,
                City = listing.City,
                State = listing.State,
                Zip = listing.Zip,
                Image = listing.Image,
                StudentId = listing.StudentId
            };
            return Ok(createdListingDto);
        }


        // GET: api/Listings/5
        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetListing([FromRoute] Guid id)
        {
            Listing? listing = await _housingPortalDbContext.Listings.FirstOrDefaultAsync(x => x.Id == id);

            if (listing == null)
            {
                return NotFound("Listing not found with the provided ID.");
            }

            ListingDto listingDto = new ListingDto
            {
                Id = listing.Id,
                Title = listing.Title,
                Description = listing.Description,
                Address = listing.Address,
                Price = listing.Price,
                City = listing.City,
                State = listing.State,
                Zip = listing.Zip,
                Image = listing.Image,
                StudentId = listing.StudentId
            };

            return Ok(listingDto);
        }

        // PUT: api/Listings/5
        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> UpdateListing([FromRoute] Guid id, [FromBody] ListingDto updateListingDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Listing? listing = await _housingPortalDbContext.Listings.FindAsync(id);

            if (listing == null)
            {
                return NotFound("Listing not found with the provided ID.");
            }

            listing.Title = updateListingDto.Title;
            listing.Description = updateListingDto.Description;
            listing.Address = updateListingDto.Address;
            listing.Price = updateListingDto.Price;
            listing.City = updateListingDto.City;
            listing.State = updateListingDto.State;
            listing.Zip = updateListingDto.Zip;
            listing.Image = updateListingDto.Image;

            await _housingPortalDbContext.SaveChangesAsync();

            ListingDto updatedListingDto = new ListingDto
            {
                Id = listing.Id,
                Title = listing.Title,
                Description = listing.Description,
                Address = listing.Address,
                Price = listing.Price,
                City = listing.City,
                State = listing.State,
                Zip = listing.Zip,
                Image = listing.Image,
                StudentId = listing.StudentId
            };

            return Ok(updatedListingDto);
        }

        // DELETE: api/Listings/5
        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> DeleteListing([FromRoute] Guid id)
        {
            Listing? listing = await _housingPortalDbContext.Listings.FindAsync(id);

            if (listing == null)
            {
                return NotFound("Listing not found with the provided ID.");
            }

            _housingPortalDbContext.Listings.Remove(listing);

            await _housingPortalDbContext.SaveChangesAsync();

            ListingDto deletedListingDto = new ListingDto
            {
                Id = listing.Id,
                Title = listing.Title,
                Description = listing.Description,
                Address = listing.Address,
                Price = listing.Price,
                City = listing.City,
                State = listing.State,
                Zip = listing.Zip,
                Image = listing.Image,
                StudentId = listing.StudentId
            };

            return Ok(deletedListingDto);
        }

        private bool ListingExists(Guid id) => _housingPortalDbContext.Listings.Any(e => e.Id == id);

    }
}
