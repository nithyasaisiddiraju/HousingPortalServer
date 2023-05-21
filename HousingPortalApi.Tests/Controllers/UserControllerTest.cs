using Xunit;
using Moq;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using HousingPortalApi.Controllers;
using HousingPortalApi.Dtos;
using HousingPortalApi.Models;
using Microsoft.AspNetCore.Mvc;
using HousingPortalApi.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using HousingPortalApi.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.FileSystemGlobbing;
using static System.Net.Mime.MediaTypeNames;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Net.NetworkInformation;
using System;
using System.Numerics;
using HousingPortalApi.Data;

namespace HousingPortalApi.Tests.Controllers
{
    public class UserControllerTest
    {
        private readonly UserController _controller;
        private readonly Mock<UserManager<HousingPortalUser>> _mockUserManager;
        private readonly Mock<IJwtHandler> _mockJwtHandler;
        private readonly Mock<IStudentService> _mockStudentService;
        private readonly Mock<IPasswordHasherWrapper> _mockPasswordHasherWrapper;
        private readonly Mock<IPasswordHasher<HousingPortalUser>> _mockPasswordHasher;

        public UserControllerTest()
        {
            _mockUserManager = new Mock<UserManager<HousingPortalUser>>(Mock.Of<IUserStore<HousingPortalUser>>(), null, null, null, null, null, null, null, null);
            _mockJwtHandler = new Mock<IJwtHandler>();
            _mockStudentService = new Mock<IStudentService>();
            _mockPasswordHasherWrapper = new Mock<IPasswordHasherWrapper>();
            _mockPasswordHasher = new Mock<IPasswordHasher<HousingPortalUser>>();

            var options = new DbContextOptionsBuilder<HousingPortalDbContext>()
            .UseInMemoryDatabase(databaseName: "Test Database")
            .Options;

            var dbContext = new HousingPortalDbContext(options);

            _controller = new UserController(
                _mockUserManager.Object,
                _mockJwtHandler.Object,
                _mockPasswordHasher.Object,
                _mockStudentService.Object,
                dbContext,
                _mockPasswordHasherWrapper.Object
            );
        }


        [Fact]
        public async Task Login_UserNotFound_ReturnsUnauthorized()
        {
            _mockJwtHandler.Setup(x => x.GetTokenAsync(It.IsAny<HousingPortalUser>(), It.IsAny<Claim[]>()))
                .ReturnsAsync(new JwtSecurityToken());

            var loginRequest = new LoginRequest { Username = "nonexistentUser", Password = "anyPassword" };

            var result = await _controller.Login(loginRequest);

            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task Login_CorrectCredentials_ReturnsOkResultWithToken()
        {
            var user = new HousingPortalUser { UserName = "existingUser" };
            var loginRequest = new LoginRequest { Username = "existingUser", Password = "correctPassword" };

            _mockUserManager.Setup(x => x.FindByNameAsync(loginRequest.Username))
                .ReturnsAsync(user);

            _mockPasswordHasherWrapper.Setup(x => x.VerifyHashedPassword(user, user.PasswordHash, loginRequest.Password))
                .Returns(PasswordVerificationResult.Success);

            _mockJwtHandler.Setup(x => x.GetTokenAsync(user, It.IsAny<Claim[]>()))
                .ReturnsAsync(new JwtSecurityToken());

            var result = await _controller.Login(loginRequest);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var loginResult = Assert.IsType<LoginResult>(okResult.Value);
            Assert.True(loginResult.Success);
            Assert.NotNull(loginResult.Token);
        }

        [Fact]
        public async Task Register_UserAlreadyExists_ReturnsBadRequest()
        {
            var registerRequest = new RegisterRequest { Username = "existingUser", Email = "email@example.com", Password = "Password123!" };

            _mockUserManager.Setup(x => x.FindByNameAsync(registerRequest.Username))
                .ReturnsAsync(new HousingPortalUser());

            var result = await _controller.Register(registerRequest);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var registerResult = Assert.IsType<RegisterResult>(badRequestResult.Value);
            Assert.False(registerResult.Success);
            Assert.Equal("Username already exists.", registerResult.Message);
        }

        [Fact]
        public async Task Register_NewUser_ReturnsOkResultWithToken()
        {
            var registerRequest = new RegisterRequest { Username = "newUser", Email = "email@example.com", Password = "Password123!", Major = "Computer Science",
                GraduationYear = 2024, Phone = "+(818)5107652"
            };

            _mockUserManager.Setup(x => x.FindByNameAsync(registerRequest.Username))
                .ReturnsAsync((HousingPortalUser)null);

            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<HousingPortalUser>()))
                .ReturnsAsync(IdentityResult.Success);

            _mockJwtHandler.Setup(x => x.GetTokenAsync(It.IsAny<HousingPortalUser>(), It.IsAny<Claim[]>()))
                .ReturnsAsync(new JwtSecurityToken());

            var result = await _controller.Register(registerRequest);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var registerResult = Assert.IsType<RegisterResult>(okResult.Value);
            Assert.True(registerResult.Success);
            Assert.NotNull(registerResult.Token);
        }

        [Fact]
        public async Task GetStudentDetails_NonExistingStudent_ReturnsNotFound()
        {
            var studentId = Guid.NewGuid();

            _mockStudentService.Setup(x => x.GetStudentDetails(studentId))
                .ReturnsAsync((StudentDto)null);

            var result = await _controller.GetStudentDetails(studentId);
            var actionResult = Assert.IsType<ActionResult<StudentDto>>(result);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async Task GetStudentDetails_ExistingStudent_ReturnsOkResultWithStudentDetails()
        {
            var studentId = Guid.NewGuid();
            StudentDto studentDto = new StudentDto { StudentId = studentId };

            _mockStudentService.Setup(x => x.GetStudentDetails(studentId))
                .ReturnsAsync(studentDto);

            var result = await _controller.GetStudentDetails(studentId);
            var actionResult = Assert.IsType<ActionResult<StudentDto>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            Assert.Equal(studentDto, okResult.Value);
        }

        [Fact]
        public async Task GetStudentListings_NonExistingStudent_ReturnsOkResultWithEmptyList()
        {
            var studentId = Guid.NewGuid();

            var result = await _controller.GetStudentListings(studentId);

            var actionResult = Assert.IsType<ActionResult<List<ListingDto>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var listingDtos = Assert.IsType<List<ListingDto>>(okResult.Value);
            Assert.Empty(listingDtos);
        }


        [Fact]
        public async Task GetStudentListings_ExistingStudent_ReturnsOkResultWithListings()
        {
            var studentId = Guid.NewGuid();
            Student student = new Student
            {
                StudentId = studentId,
                Name = "Nithyasai",
                Email = "test@student.com",
                Phone = "1234567890",
                Major = "Computer Science",
                GraduationYear = 2024
            };

            _controller._housingPortalDbContext.Students.Add(student);
            _controller._housingPortalDbContext.SaveChanges();

            var expectedListings = new List<Listing>
            {
                new Listing
                {
                    StudentId = studentId,
                    ListingId = Guid.NewGuid(),
                    Title = "Spacious 2-bedroom apartment",
                    Description = "A comfortable 2-bedroom apartment near the city center.",
                    Address = "123 Main Street",
                    Price = 1200,
                    City = "Los Angeles",
                    State = "CA",
                    Zip = "12345",
                    Image = "https://example.com/apartment.jpg",
                    Student = student
                }
            };

            _controller._housingPortalDbContext.Listings.AddRange(expectedListings);
            _controller._housingPortalDbContext.SaveChanges();
            var result = await _controller.GetStudentListings(studentId);

            var actionResult = Assert.IsType<ActionResult<List<ListingDto>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var listingDtos = Assert.IsType<List<ListingDto>>(okResult.Value);
            Assert.NotEmpty(listingDtos);
        }

    }
}
