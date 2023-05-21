using HousingPortalApi.Data;
using HousingPortalApi.Dtos;
using HousingPortalApi.Models;
using HousingPortalApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HousingPortalApi.Interfaces;

namespace HousingPortalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<HousingPortalUser> _userManager;
        private readonly IJwtHandler _jwtHandler;
        private readonly IStudentService _studentService;
        private readonly Regex passwordCheck = new Regex("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[!@#$%^&*()_+=])[A-Za-z\\d!@#$%^&*()_+=]{8,}$");
        public HousingPortalDbContext _housingPortalDbContext;
        private readonly IPasswordHasherWrapper _passwordHasherWrapper;
        public UserController(UserManager<HousingPortalUser> userManager, IJwtHandler jwtHandler, IPasswordHasher<HousingPortalUser> passwordHasher,
        IStudentService studentService, HousingPortalDbContext housingPortalDbContext, IPasswordHasherWrapper passwordHasherWrapper)
        {
            _userManager = userManager;
            _jwtHandler = jwtHandler;
            _passwordHasherWrapper = passwordHasherWrapper;
            _studentService = studentService;
            _housingPortalDbContext = housingPortalDbContext;
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            HousingPortalUser? user = await _userManager.FindByNameAsync(loginRequest.Username);

            if (user == null)
            {
                return Unauthorized(new LoginResult { Success = false, Message = "User not found." });
            }

            if (_passwordHasherWrapper.VerifyHashedPassword(user, user.PasswordHash, loginRequest.Password) != PasswordVerificationResult.Success)
            {
                return Unauthorized(new LoginResult { Success = false, Message = "Incorrect password." });
            }
            JwtSecurityToken secToken = await _jwtHandler.GetTokenAsync(user, new Claim[] { new Claim(ClaimTypes.NameIdentifier, user.Id) });

            string? jwt = new JwtSecurityTokenHandler().WriteToken(secToken);

            return Ok(new LoginResult { Success = true, Message = "Login successful", Token = jwt });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest registerRequest)
        {
            if (await _userManager.FindByNameAsync(registerRequest.Username) != null)
            {
                return BadRequest(new RegisterResult { Success = false, Message = "Username already exists." });
            }

            if (await _userManager.FindByEmailAsync(registerRequest.Email) != null)
            {
                return BadRequest(new RegisterResult { Success = false, Message = "Email already exists." });
            }

            if (!passwordCheck.IsMatch(registerRequest.Password))
            {
                return BadRequest(new RegisterResult { Success = false, Message = "Password is too weak. It should be at least 8 characters long, include an uppercase letter, a lowercase letter, a number, and a special character." });
            }

            HousingPortalUser user = new HousingPortalUser { UserName = registerRequest.Username, Email = registerRequest.Email };
            user.PasswordHash = _passwordHasherWrapper.HashPassword(user, registerRequest.Password);
            var result = await _userManager.CreateAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest(new RegisterResult {Success = false, Message = "Registration failed. Try again." });
            }

            Student registeredStudent = new Student
            {
                StudentId = Guid.Parse(user.Id),
                Name = registerRequest.Username,
                Email = registerRequest.Email,
                Phone = registerRequest.Phone,
                Major = registerRequest.Major,
                GraduationYear = registerRequest.GraduationYear
            };

            _housingPortalDbContext.Students.Add(registeredStudent);
            await _housingPortalDbContext.SaveChangesAsync();

            JwtSecurityToken secToken = await _jwtHandler.GetTokenAsync(user, new Claim[] { new Claim(ClaimTypes.NameIdentifier, user.Id) });
            string? jwt = new JwtSecurityTokenHandler().WriteToken(secToken);

            return Ok(new RegisterResult { Success = true, Message = "Registration successful", Token = jwt });


        }

        [HttpGet("{studentId}")]
        public async Task<ActionResult<StudentDto>> GetStudentDetails(Guid studentId)
        {
            var studentDto = await _studentService.GetStudentDetails(studentId);
            if (studentDto == null)
            {
                return NotFound();
            }
            return Ok(studentDto);
        }

        [HttpGet("{studentId}/listings")]
        public async Task<ActionResult<List<ListingDto>>> GetStudentListings(Guid studentId)
        {
            var listings = await _housingPortalDbContext.Listings
                .Include(l => l.Student)
                .Where(l => l.StudentId == studentId)
                .ToListAsync();

            var listingDtos = listings.Select(l => new ListingDto
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
                StudentDto = new StudentDto
                {
                    StudentId = l.Student.StudentId,
                    Name = l.Student.Name,
                    Email = l.Student.Email,
                    Phone = l.Student.Phone,
                    Major = l.Student.Major,
                    GraduationYear = l.Student.GraduationYear
                }
            }).ToList();

            return Ok(listingDtos);
        }
    }
}