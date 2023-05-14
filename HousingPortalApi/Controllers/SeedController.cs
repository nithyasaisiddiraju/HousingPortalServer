using HousingPortalApi.Dtos;
using HousingPortalApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace HousingPortalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeedController : ControllerBase
    {
        private readonly UserManager<HousingPortalUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly HousingPortalDbContext _context;
        private readonly string _pathName;

        public SeedController(UserManager<HousingPortalUser> userManager, RoleManager<IdentityRole> roleManager,
            IConfiguration configuration, HousingPortalDbContext context, IHostEnvironment environment)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _context = context;
            _pathName = Path.Combine(environment.ContentRootPath, "Data/StudentListings.csv");
        }

        [HttpPost("Listings")]
        public async Task<IActionResult> ImportListings()
        {
            Console.WriteLine("ImportListings started");
            Dictionary<Guid, Listing> listingsById = _context.Listings.AsNoTracking()
                .ToDictionary(x => x.Id, x => x);

            Dictionary<Guid, Student> studentsById = _context.Students.AsNoTracking()
                .ToDictionary(x => x.Id, x => x);

            CsvConfiguration config = new(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                HeaderValidated = null,
            };

            using StreamReader reader = new(_pathName);
            using CsvReader csv = new(reader, config);

            IEnumerable<StudentListingDto> records = csv.GetRecords<StudentListingDto>();

            List<Listing> importedListings = new List<Listing>();

            foreach (StudentListingDto record in records)
            {
                Console.WriteLine($"Processing record with ID: {record.Id}");

                if (!listingsById.ContainsKey(record.Id) && !importedListings.Any(l => l.Id == record.Id))
                {
                    var student = studentsById.GetValueOrDefault(record.StudentId);

                    if (student == null)
                    {
                        student = new Student
                        {
                            Id = record.StudentId,
                            Name = record.Name,
                            Email = record.Email,
                            Phone = record.Phone,
                            Major = record.Major,
                            GraduationYear = record.GraduationYear
                        };
                        await _context.Students.AddAsync(student);
                        studentsById.Add(student.Id, student);
                    }

                    var listing = new Listing
                    {
                        Id = record.Id,
                        Title = record.Title,
                        Description = record.Description,
                        Address = record.Address,
                        Price = record.Price,
                        City = record.City,
                        State = record.State,
                        Zip = record.Zip,
                        Image = record.Image,
                        StudentId = student.Id
                    };

                    Console.WriteLine($"Adding new listing with ID: {record.Id}");
                    importedListings.Add(listing);
                }
                else
                {
                    Console.WriteLine($"Listing with ID {record.Id} already exists");
                }
            }

            _context.Listings.AddRange(importedListings);
            await _context.SaveChangesAsync();
            Console.WriteLine("ImportListings finished");

            return Ok(importedListings.Select(l => new ListingDto
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
            }));
        }


        [HttpPost("Users")]
        public async Task<IActionResult> ImportUsers()
        {
            const string roleUser = "RegisteredUser";
            const string roleAdmin = "Administrator";

            if (await _roleManager.FindByNameAsync(roleUser) is null)
            {
                await _roleManager.CreateAsync(new IdentityRole(roleUser));
            }
            if (await _roleManager.FindByNameAsync(roleAdmin) is null)
            {
                await _roleManager.CreateAsync(new IdentityRole(roleAdmin));
            }

            List<HousingPortalUser> addedUserList = new();
            (string name, string email) = ("admin", "admin@email.com");

            if (await _userManager.FindByNameAsync(name) is null)
            {
                HousingPortalUser userAdmin = new()
                {
                    UserName = name,
                    Email = email,
                    SecurityStamp = Guid.NewGuid().ToString()
                };
                await _userManager.CreateAsync(userAdmin, _configuration["DefaultPasswords:Administrator"]
                    ?? throw new InvalidOperationException());
                await _userManager.AddToRolesAsync(userAdmin, new[] { roleUser, roleAdmin });
                userAdmin.EmailConfirmed = true;
                userAdmin.LockoutEnabled = false;
                addedUserList.Add(userAdmin);
            }

            (string name, string email) registered = ("user", "user@email.com");

            if (await _userManager.FindByNameAsync(registered.name) is null)
            {
                HousingPortalUser user = new()
                {
                    UserName = registered.name,
                    Email = registered.email,
                    SecurityStamp = Guid.NewGuid().ToString()
                };
                await _userManager.CreateAsync(user, _configuration["DefaultPasswords:RegisteredUser"]
                    ?? throw new InvalidOperationException());
                await _userManager.AddToRoleAsync(user, roleUser);
                user.EmailConfirmed = true;
                user.LockoutEnabled = false;
                addedUserList.Add(user);
            }

            if (addedUserList.Count > 0)
            {
                await _context.SaveChangesAsync();
            }

            return new JsonResult(new
            {
                addedUserList.Count,
                Users = addedUserList
            });

        }
    }
}