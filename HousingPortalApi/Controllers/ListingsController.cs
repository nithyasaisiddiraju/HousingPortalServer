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
            List<Listing> listings = await _housingPortalDbContext.Listings.Include(l => l.Student).ToListAsync();

            List<ListingDto> listingDtos = listings.Select(l => new ListingDto
            {
                ListingId = l.ListingId,
                Title = l.Title,
                Description = l.Description,
                Address = l.Address,
                Price = l.Price,
                City = l.City,
                State = l.State,
                Zip = l.Zip,
                Image = l.Image,
                StudentDto = l.Student != null ? new StudentDto
                {
                    StudentId = l.Student.StudentId,
                    Name = l.Student.Name,
                    Email = l.Student.Email,
                    Phone = l.Student.Phone,
                    Major = l.Student.Major,
                    GraduationYear = l.Student.GraduationYear
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
                .FirstOrDefaultAsync(l => l.Title == listingDto.Title &&
                                          l.Description == listingDto.Description &&
                                          l.Address == listingDto.Address);

            if (existingListing != null)
            {
                Console.WriteLine("Listing already exists with the same title, description, and address");
                return BadRequest(new { message = "Listing already exists with the same title, description, and address" });
            }

            Listing listing = new Listing
            {
                ListingId = Guid.NewGuid(),
                Title = listingDto.Title,
                Description = listingDto.Description,
                Address = listingDto.Address,
                Price = listingDto.Price,
                City = listingDto.City,
                State = listingDto.State,
                Zip = listingDto.Zip,
                Image = "https://images.pexels.com/photos/106399/pexels-photo-106399.jpeg",
                Student = studentResult
            };

            _housingPortalDbContext.Listings.Add(listing);
            await _housingPortalDbContext.SaveChangesAsync();

            StudentDto studentDto = new StudentDto
            {
                StudentId = listing.Student.StudentId,
                Name = listing.Student.Name,
                Email = listing.Student.Email,
                Phone = listing.Student.Phone,
                Major = listing.Student.Major,
                GraduationYear = listing.Student.GraduationYear

            };

            ListingDto createdListingDto = new ListingDto
            {
                ListingId = listing.ListingId,
                Title = listing.Title,
                Description = listing.Description,
                Address = listing.Address,
                Price = listing.Price,
                City = listing.City,
                State = listing.State,
                Zip = listing.Zip,
                Image = listing.Image,
                StudentDto = studentDto
            };
            return Ok(createdListingDto);
        }


        // GET: api/Listings/5
        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetListing([FromRoute] Guid id)
        {
            Listing? listing = await _housingPortalDbContext.Listings
           .Include(l => l.Student)
           .FirstOrDefaultAsync(x => x.ListingId == id);

            if (listing == null)
            {
                return NotFound("Listing not found with the provided ID.");
            }

            StudentDto studentDto = new StudentDto
            {
                StudentId = listing.Student.StudentId,
                Name = listing.Student.Name,
                Email = listing.Student.Email,
                Phone = listing.Student.Phone,
                Major = listing.Student.Major,
                GraduationYear = listing.Student.GraduationYear
            };

            ListingDto listingDto = new ListingDto
            {
                ListingId = listing.ListingId,
                Title = listing.Title,
                Description = listing.Description,
                Address = listing.Address,
                Price = listing.Price,
                City = listing.City,
                State = listing.State,
                Zip = listing.Zip,
                Image = "https://images.pexels.com/photos/106399/pexels-photo-106399.jpeg",
                StudentDto = studentDto
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
                .Include(l => l.Student)
                .FirstOrDefaultAsync(l => l.ListingId == id);

            if (listing == null)
            {
                return NotFound("Listing not found with the provided ID.");
            }

            if (listing.StudentId != studentId)
            {
                return BadRequest("You do not have permission to update this listing.");
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
                ListingId = listing.ListingId,
                Title = listing.Title,
                Description = listing.Description,
                Address = listing.Address,
                Price = listing.Price,
                City = listing.City,
                State = listing.State,
                Zip = listing.Zip,
                Image = listing.Image,
                StudentDto = new StudentDto
                {
                    StudentId = listing.Student.StudentId,
                    Name = listing.Student.Name,
                    Email = listing.Student.Email,
                    Phone = listing.Student.Phone,
                    Major = listing.Student.Major,
                    GraduationYear = listing.Student.GraduationYear
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
                .Include(l => l.Student)
                .FirstOrDefaultAsync(x => x.ListingId == id);

            if (listing == null)
            {
                return NotFound("Listing not found with the provided ID.");
            }

            if (listing.StudentId != studentId)
            {
                return BadRequest("You do not have permission to delete this listing.");
            }

            _housingPortalDbContext.Listings.Remove(listing);

            await _housingPortalDbContext.SaveChangesAsync();

            ListingDto deletedListingDto = new ListingDto
            {
                ListingId = listing.ListingId,
                Title = listing.Title,
                Description = listing.Description,
                Address = listing.Address,
                Price = listing.Price,
                City = listing.City,
                State = listing.State,
                Zip = listing.Zip,
                Image = listing.Image,
                StudentDto = new StudentDto
                {
                    StudentId = listing.Student.StudentId,
                    Name = listing.Student.Name,
                    Email = listing.Student.Email,
                    Phone = listing.Student.Phone,
                    Major = listing.Student.Major,
                    GraduationYear = listing.Student.GraduationYear
                }
            };

            return Ok(deletedListingDto);
        }
    }
}