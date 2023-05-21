using HousingPortalApi.Dtos;
using HousingPortalApi.Interfaces;
using HousingPortalApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static System.Net.Mime.MediaTypeNames;

namespace HousingPortalApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ListingsController : ControllerBase
    {
        private readonly IHousingPortalDbContext _housingPortalDbContext;

        public ListingsController(IHousingPortalDbContext housingPortalDbContext)
        {
            _housingPortalDbContext = housingPortalDbContext;
        }

        // GET: api/Listings
        [HttpGet]
        public async Task<IActionResult> GetAllListings()
        {
            List<Listing> listings = await _housingPortalDbContext.Listings.Include(l => l.student).ToListAsync();

            List<ListingDto> listingDtos = listings.Select(l => new ListingDto
            {
                listingId = l.listingId,
                title = l.title,
                description = l.description,
                address = l.address,
                price = l.price,
                city = l.city,
                state = l.state,
                zip = l.zip,
                image = l.image,
                studentDto = l.student != null ? new StudentDto
                {
                    studentId = l.student.studentId,
                    name = l.student.name,
                    email = l.student.email,
                    phone = l.student.phone,
                    major = l.student.major,
                    graduationYear = l.student.graduationYear
                } : null
            }).ToList();

            return Ok(listingDtos);
        }

        // POST: api/Listings
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddListing([FromBody] ListingDto listingDto)
        {
            Console.WriteLine("AddListing called with ListingDto: {0}", listingDto);

            string loggedInStudentId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            Console.WriteLine("LoggedInStudentId: {0}", loggedInStudentId);

            if (string.IsNullOrEmpty(loggedInStudentId))
            {
                Console.WriteLine("No student found with the given ID");
                return BadRequest(new { message = "No student found with the given ID" });
            }

            Guid studentId;
            if (!Guid.TryParse(loggedInStudentId, out studentId))
            {
                Console.WriteLine("Invalid student ID: {0}", loggedInStudentId);
                return BadRequest(new { message = "Invalid student ID" });
            }

            Student studentResult = await _housingPortalDbContext.Students.FindAsync(studentId);
            if (studentResult == null)
            {
                Console.WriteLine("No student found with the ID: {0}", studentId);
                return NotFound(new { message = $"No student found with ID: {studentId}" });
            }

            var existingListing = await _housingPortalDbContext.Listings
                .FirstOrDefaultAsync(l => l.title == listingDto.title &&
                                          l.description == listingDto.description &&
                                          l.address == listingDto.address);

            if (existingListing != null)
            {
                Console.WriteLine("Listing already exists with the same title, description, and address");
                return BadRequest(new { message = "Listing already exists with the same title, description, and address" });
            }

            Listing listing = new Listing
            {
                listingId = Guid.NewGuid(),
                title = listingDto.title,
                description = listingDto.description,
                address = listingDto.address,
                price = listingDto.price,
                city = listingDto.city,
                state = listingDto.state,
                zip = listingDto.zip,
                image = "https://images.pexels.com/photos/106399/pexels-photo-106399.jpeg",
                student = studentResult
            };

            _housingPortalDbContext.Listings.Add(listing);
            await _housingPortalDbContext.SaveChangesAsync();

            StudentDto studentDto = new StudentDto
            {
                studentId = listing.student.studentId,
                name = listing.student.name,
                email = listing.student.email,
                phone = listing.student.phone,
                major = listing.student.major,
                graduationYear = listing.student.graduationYear

            };

            ListingDto createdListingDto = new ListingDto
            {
                listingId = listing.listingId,
                title = listing.title,
                description = listing.description,
                address = listing.address,
                price = listing.price,
                city = listing.city,
                state = listing.state,
                zip = listing.zip,
                image = listing.image,
                studentDto = studentDto
            };
            return Ok(createdListingDto);
        }


        // GET: api/Listings/5
        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetListing([FromRoute] Guid id)
        {
            Listing? listing = await _housingPortalDbContext.Listings
           .Include(l => l.student)
           .FirstOrDefaultAsync(x => x.listingId == id);

            if (listing == null)
            {
                return NotFound("Listing not found with the provided ID.");
            }

            StudentDto studentDto = new StudentDto
            {
                studentId = listing.student.studentId,
                name = listing.student.name,
                email = listing.student.email,
                phone = listing.student.phone,
                major = listing.student.major,
                graduationYear = listing.student.graduationYear
            };

            ListingDto listingDto = new ListingDto
            {
                listingId = listing.listingId,
                title = listing.title,
                description = listing.description,
                address = listing.address,
                price = listing.price,
                city = listing.city,
                state = listing.state,
                zip = listing.zip,
                image = "https://images.pexels.com/photos/106399/pexels-photo-106399.jpeg",
                studentDto = studentDto
            };

            return Ok(listingDto);
        }

        // PUT: api/Listings/5
        [HttpPut]
        [Route("{id:Guid}")]
        [Authorize]
        public async Task<IActionResult> UpdateListing([FromRoute] Guid id, [FromBody] ListingDto updateListingDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string loggedInStudentId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            Guid studentId;
            if (!Guid.TryParse(loggedInStudentId, out studentId))
            {
                Console.WriteLine("Invalid student ID: {0}", loggedInStudentId);
                return BadRequest(new { message = "Invalid student ID" });
            }

            Listing? listing = await _housingPortalDbContext.Listings
                .Include(l => l.student)
                .FirstOrDefaultAsync(l => l.listingId == id);

            if (listing == null)
            {
                return NotFound("Listing not found with the provided ID.");
            }

            if (listing.studentId != studentId)
            {
                return BadRequest("You do not have permission to update this listing.");
            }

            listing.title = updateListingDto.title;
            listing.description = updateListingDto.description;
            listing.address = updateListingDto.address;
            listing.price = updateListingDto.price;
            listing.city = updateListingDto.city;
            listing.state = updateListingDto.state;
            listing.zip = updateListingDto.zip;
            listing.image = updateListingDto.image;

            await _housingPortalDbContext.SaveChangesAsync();

            ListingDto updatedListingDto = new ListingDto
            {
                listingId = listing.listingId,
                title = listing.title,
                description = listing.description,
                address = listing.address,
                price = listing.price,
                city = listing.city,
                state = listing.state,
                zip = listing.zip,
                image = listing.image,
                studentDto = new StudentDto
                {
                    studentId = listing.student.studentId,
                    name = listing.student.name,
                    email = listing.student.email,
                    phone = listing.student.phone,
                    major = listing.student.major,
                    graduationYear = listing.student.graduationYear
                }
            };

            return Ok(updatedListingDto);
        }

        // DELETE: api/Listings/5
        [HttpDelete]
        [Route("{id:Guid}")]
        [Authorize]
        public async Task<IActionResult> DeleteListing([FromRoute] Guid id)
        {
            string loggedInStudentId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            Guid studentId;
            if (!Guid.TryParse(loggedInStudentId, out studentId))
            {
                Console.WriteLine("Invalid student ID: {0}", loggedInStudentId);
                return BadRequest(new { message = "Invalid student ID" });
            }

            Listing? listing = await _housingPortalDbContext.Listings
                .Include(l => l.student)
                .FirstOrDefaultAsync(x => x.listingId == id);

            if (listing == null)
            {
                return NotFound("Listing not found with the provided ID.");
            }

            if (listing.studentId != studentId)
            {
                return BadRequest("You do not have permission to delete this listing.");
            }

            _housingPortalDbContext.Listings.Remove(listing);

            await _housingPortalDbContext.SaveChangesAsync();

            ListingDto deletedListingDto = new ListingDto
            {
                listingId = listing.listingId,
                title = listing.title,
                description = listing.description,
                address = listing.address,
                price = listing.price,
                city = listing.city,
                state = listing.state,
                zip = listing.zip,
                image = listing.image,
                studentDto = new StudentDto
                {
                    studentId = listing.student.studentId,
                    name = listing.student.name,
                    email = listing.student.email,
                    phone = listing.student.phone,
                    major = listing.student.major,
                    graduationYear = listing.student.graduationYear
                }
            };

            return Ok(deletedListingDto);
        }
    }
}