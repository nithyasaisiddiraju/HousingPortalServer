using HousingPortalApi.Controllers;
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

        var student = new Student
        {
            studentId = Guid.Parse("3F0D6B98-4809-455D-85F0-464188966267"),
            name = "Nithyasai",
            email = "listing@example.com",
            phone = "(213) 555-9997",
            major = "Computer Science",
            graduationYear = 2025
        };

        var listing = new Listing
        {
            listingId = Guid.Parse("1A7933C3-B50D-48FA-83A7-8874FF0B099E"),
            title = "Spacious 2-bedroom apartment",
            description = "A comfortable 2-bedroom apartment near the city center.",
            address = "123 Main Street",
            price = 1200,
            city = "LA",
            state = "CA",
            zip = "12345",
            image = "https://example.com/apartment.jpg",
            student = student
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
        var listingDto = new ListingDto
        {
            title = "Luxury Condo",
            description = "Elegant condo with amenities",
            address = "988 Cherry Lane",
            price = 1600,
            city = "Los Angeles",
            state = "CA",
            zip = "678345",
            image = "https://example.com/new_listing.jpg",
            studentDto = new StudentDto
            {
                studentId = _students.First().studentId,
                name = _students.First().name,
                email = _students.First().email,
                phone = _students.First().phone,
                major = _students.First().major,
                graduationYear = _students.First().graduationYear
            }
        };

        var result = await _listingsController.AddListing(listingDto);
        Assert.IsType<OkObjectResult>(result);
        var objectResult = result as OkObjectResult;

        var returnedListing = objectResult.Value as ListingDto;
        Assert.NotNull(returnedListing);
        Assert.Equal(listingDto.title, returnedListing.title);
        Assert.Equal(listingDto.description, returnedListing.description);
        Assert.Equal(listingDto.address, returnedListing.address);
        Assert.Equal(listingDto.price, returnedListing.price);
        Assert.Equal(listingDto.city, returnedListing.city);
        Assert.Equal(listingDto.state, returnedListing.state);
        Assert.Equal(listingDto.zip, returnedListing.zip);
        Assert.Equal(listingDto.studentDto.studentId, returnedListing.studentDto.studentId);

        var dbListing = await _housingPortalDbContext.Listings.Include(l => l.student).FirstOrDefaultAsync(l => l.listingId == returnedListing.listingId);
        Assert.NotNull(dbListing);
    }

    [Fact]
    public async Task GetListing_WithExistingId_ShouldReturnListing()
    {
        Guid existingId = _listings.First().listingId;
        var result = await _listingsController.GetListing(existingId);

        Assert.IsType<OkObjectResult>(result);
        var objectResult = result as OkObjectResult;
        var returnedListing = objectResult.Value as ListingDto;
        Assert.NotNull(returnedListing);
        Assert.Equal(existingId, returnedListing.listingId);
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
        Guid existingId = _listings.First().listingId;
        var updateListingDto = new ListingDto
        {
            title = "Spacious 4-bedroom apartment",
            description = "A comfortable 2-bedroom apartment near the city center.",
            address = "123 Main Street",
            price = 1200,
            city = "LA",
            state = "CA",
            zip = "12345",
            image = "https://example.com/apartment.jpg",
        };

        var result = await _listingsController.UpdateListing(existingId, updateListingDto);

        Assert.IsType<OkObjectResult>(result);
        var objectResult = result as OkObjectResult;
        var updatedListing = objectResult.Value as ListingDto;
        Assert.Equal(updateListingDto.title, updatedListing.title);
        Assert.Equal(updateListingDto.description, updatedListing.description);
        Assert.Equal(updateListingDto.address, updatedListing.address);
        Assert.Equal(updateListingDto.price, updatedListing.price);
        Assert.Equal(updateListingDto.city, updatedListing.city);
        Assert.Equal(updateListingDto.state, updatedListing.state);
        Assert.Equal(updateListingDto.zip, updatedListing.zip);
        Assert.Equal(updateListingDto.image, updatedListing.image);
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
        Guid existingId = _listings.First().listingId;
        var result = await _listingsController.DeleteListing(existingId);
        Assert.IsType<OkObjectResult>(result);
        var deleted = _housingPortalDbContext.Listings.Any(l => l.listingId == existingId);
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
