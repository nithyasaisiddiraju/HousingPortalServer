using HousingPortalApi.Controllers;
using HousingPortalApi.Data;
using HousingPortalApi.Dtos;
using HousingPortalApi.Interfaces;
using HousingPortalApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

public class ListingsControllerTest
{
    private readonly HousingPortalDbContext _housingPortalDbContext;
    private readonly ListingsController _listingsController;
    private readonly List<Listing> _listings;
    private readonly List<Student> _students;
    private readonly ClaimsPrincipal _user;

    public ListingsControllerTest()
    {
        var options = new DbContextOptionsBuilder<HousingPortalDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _housingPortalDbContext = new HousingPortalDbContext(options);

        Student student = new Student
        {
            StudentId = Guid.Parse("3F0D6B98-4809-455D-85F0-464188966267"),
            Name = "Nithyasai",
            Email = "listing@example.com",
            Phone = "(213) 555-9997",
            Major = "Computer Science",
            GraduationYear = 2025
        };

        Listing listing = new Listing
        {
            ListingId = Guid.Parse("1A7933C3-B50D-48FA-83A7-8874FF0B099E"),
            Title = "Spacious 2-bedroom apartment",
            Description = "A comfortable 2-bedroom apartment near the city center.",
            Address = "123 Main Street",
            Price = 1200,
            City = "LA",
            State = "CA",
            Zip = "12345",
            Image = "https://example.com/apartment.jpg",
            Student = student
        };

        _students = new List<Student> { student };
        _listings = new List<Listing> { listing };

        _housingPortalDbContext.Students.AddRange(_students);
        _housingPortalDbContext.Listings.AddRange(_listings);
        _housingPortalDbContext.SaveChanges();

        _user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, "3F0D6B98-4809-455D-85F0-464188966267")
        }));

        _listingsController = new ListingsController(_housingPortalDbContext);
        _listingsController.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext() { User = _user }
        };
    }


    [Fact]
    public async Task GetAllListings_ShouldReturnOkObjectResult()
    {
        var result = await _listingsController.GetAllListings();
        Assert.IsType<OkObjectResult>(result);
        var objectResult = result as OkObjectResult;
        Assert.Equal(_listings.Count, ((List<ListingDto>)objectResult.Value).Count);
    }

    [Fact]
    public async Task AddListing_ShouldReturnOkObjectResult()
    {
        ListingDto listingDto = new ListingDto
        {
            Title = "Luxury Condo",
            Description = "Elegant condo with amenities",
            Address = "988 Cherry Lane",
            Price = 1600,
            City = "Los Angeles",
            State = "CA",
            Zip = "678345",
            Image = "https://example.com/new_listing.jpg",
            StudentDto = new StudentDto
            {
                StudentId = _students.First().StudentId,
                Name = _students.First().Name,
                Email = _students.First().Email,
                Phone = _students.First().Phone,
                Major = _students.First().Major,
                GraduationYear = _students.First().GraduationYear
            }
        };

        var result = await _listingsController.AddListing(listingDto);
        Assert.IsType<OkObjectResult>(result);
        var objectResult = result as OkObjectResult;

        var returnedListing = objectResult.Value as ListingDto;
        Assert.NotNull(returnedListing);
        Assert.Equal(listingDto.Title, returnedListing.Title);
        Assert.Equal(listingDto.Description, returnedListing.Description);
        Assert.Equal(listingDto.Address, returnedListing.Address);
        Assert.Equal(listingDto.Price, returnedListing.Price);
        Assert.Equal(listingDto.City, returnedListing.City);
        Assert.Equal(listingDto.State, returnedListing.State);
        Assert.Equal(listingDto.Zip, returnedListing.Zip);
        Assert.Equal(listingDto.StudentDto.StudentId, returnedListing.StudentDto.StudentId);

        var dbListing = await _housingPortalDbContext.Listings.Include(l => l.Student).FirstOrDefaultAsync(l => l.ListingId == returnedListing.ListingId);
        Assert.NotNull(dbListing);
    }

    [Fact]
    public async Task GetListing_WithExistingId_ShouldReturnListing()
    {
        Guid existingId = _listings.First().ListingId;
        var result = await _listingsController.GetListing(existingId);

        Assert.IsType<OkObjectResult>(result);
        var objectResult = result as OkObjectResult;
        var returnedListing = objectResult.Value as ListingDto;
        Assert.NotNull(returnedListing);
        Assert.Equal(existingId, returnedListing.ListingId);
    }

    [Fact]
    public async Task GetListing_WithNonexistentId_ShouldReturnNotFound()
    {
        Guid nonexistentId = Guid.NewGuid();
        var result = await _listingsController.GetListing(nonexistentId);
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task UpdateListing_WithExistingId_ShouldUpdateAndReturnListing()
    {
        Guid existingId = _listings.First().ListingId;
        var updateListingDto = new ListingDto
        {
            Title = "Spacious 4-bedroom apartment",
            Description = "A comfortable 2-bedroom apartment near the city center.",
            Address = "123 Main Street",
            Price = 1200,
            City = "LA",
            State = "CA",
            Zip = "12345",
            Image = "https://example.com/apartment.jpg",
        };

        var result = await _listingsController.UpdateListing(existingId, updateListingDto);

        Assert.IsType<OkObjectResult>(result);
        var objectResult = result as OkObjectResult;
        var updatedListing = objectResult.Value as ListingDto;
        Assert.Equal(updateListingDto.Title, updatedListing.Title);
        Assert.Equal(updateListingDto.Description, updatedListing.Description);
        Assert.Equal(updateListingDto.Address, updatedListing.Address);
        Assert.Equal(updateListingDto.Price, updatedListing.Price);
        Assert.Equal(updateListingDto.City, updatedListing.City);
        Assert.Equal(updateListingDto.State, updatedListing.State);
        Assert.Equal(updateListingDto.Zip, updatedListing.Zip);
        Assert.Equal(updateListingDto.Image, updatedListing.Image);
    }

    [Fact]
    public async Task UpdateListing_WithNonexistentId_ShouldReturnNotFound()
    {
        Guid nonexistentId = Guid.NewGuid();
        var updateListingDto = new ListingDto();
        var result = await _listingsController.UpdateListing(nonexistentId, updateListingDto);
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task DeleteListing_WithExistingId_ShouldDeleteListing()
    {
        Guid existingId = _listings.First().ListingId;
        var result = await _listingsController.DeleteListing(existingId);
        Assert.IsType<OkObjectResult>(result);
        var deleted = _housingPortalDbContext.Listings.Any(l => l.ListingId == existingId);
        Assert.False(deleted);
    }

    [Fact]
    public async Task DeleteListing_WithNonexistentId_ShouldReturnNotFound()
    {
        Guid nonexistentId = Guid.NewGuid();
        var result = await _listingsController.DeleteListing(nonexistentId);
        Assert.IsType<NotFoundObjectResult>(result);
    }

}
