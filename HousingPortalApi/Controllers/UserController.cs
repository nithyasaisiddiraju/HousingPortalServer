using HousingPortalApi.Data;
using HousingPortalApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HousingPortalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<HousingPortalUser> _userManager;
        private readonly JwtHandler _jwtHandler;
        private readonly PasswordHasher<HousingPortalUser> _passwordHasher;

        private readonly Regex passwordCheck = new Regex("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[!@#$%^&*()_+=])[A-Za-z\\d!@#$%^&*()_+=]{8,}$");

        public UserController(UserManager<HousingPortalUser> userManager, JwtHandler jwtHandler, PasswordHasher<HousingPortalUser> passwordHasher)
        {
            _userManager = userManager;
            _jwtHandler = jwtHandler;
            _passwordHasher = passwordHasher;
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            var user = await _userManager.FindByNameAsync(loginRequest.Username);

            if (user == null)
            {
                return Unauthorized(new LoginResult { Success = false, Message = "User not found." });
            }

            if (_passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginRequest.Password) != PasswordVerificationResult.Success)
            {
                return Unauthorized(new LoginResult { Success = false, Message = "Incorrect password." });
            }

            var secToken = await _jwtHandler.GetTokenAsync(user);
            var jwt = new JwtSecurityTokenHandler().WriteToken(secToken);

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

            var user = new HousingPortalUser { UserName = registerRequest.Username, Email = registerRequest.Email };
            user.PasswordHash = _passwordHasher.HashPassword(user, registerRequest.Password);
            var result = await _userManager.CreateAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest(new RegisterResult { Success = false, Message = "Registration failed. Try again." });
            }

            var secToken = await _jwtHandler.GetTokenAsync(user);
            var jwt = new JwtSecurityTokenHandler().WriteToken(secToken);

            return Ok(new RegisterResult { Success = true, Message = "Registration successful", Token = jwt });
        }
    }
}