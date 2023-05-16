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

namespace HousingPortalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<HousingPortalUser> _userManager;
        private readonly JwtHandler _jwtHandler;
        private readonly PasswordHasher<HousingPortalUser> _passwordHasher;
        private readonly StudentService _studentService;
        private readonly Regex passwordCheck = new Regex("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[!@#$%^&*()_+=])[A-Za-z\\d!@#$%^&*()_+=]{8,}$");
        private readonly HousingPortalDbContext _housingPortalDbContext;
        public UserController(UserManager<HousingPortalUser> userManager, JwtHandler jwtHandler, PasswordHasher<HousingPortalUser> passwordHasher,
            StudentService studentService, HousingPortalDbContext housingPortalDbContext)
        {
            _userManager = userManager;
            _jwtHandler = jwtHandler;
            _passwordHasher = passwordHasher;
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

            if (_passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginRequest.Password) != PasswordVerificationResult.Success)
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
            user.PasswordHash = _passwordHasher.HashPassword(user, registerRequest.Password);
            var result = await _userManager.CreateAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest(new RegisterResult { Success = false, Message = "Registration failed. Try again." });
            }

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
                .Include(l => l.student)
                .Where(l => l.studentId == studentId)
                .ToListAsync();

            var listingDtos = listings.Select(l => new ListingDto
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
                studentDto = new StudentDto
                {
                    studentId = l.student.studentId,
                    name = l.student.name,
                    email = l.student.email,
                    phone = l.student.phone,
                    major = l.student.major,
                    graduationYear = l.student.graduationYear
                }
            }).ToList();

            return Ok(listingDtos);
        }
    }
}