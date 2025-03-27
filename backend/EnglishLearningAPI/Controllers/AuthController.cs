using Microsoft.AspNetCore.Mvc;

using BCrypt.Net;   

using EnglishLearningAPI.Models;
using EnglishLearningAPI.Services;

namespace EnglishLearningAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly UserService _userService;
    
        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest login)
        {
            var token = await _authService.Authenticate(login.Email, login.Password);
            if (token == null)
                return Unauthorized(new { Message = "Invalid credentials" });
            return Ok(new { token });
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]  User user)
        {
            if (await _userService.GetUserByEmailAsync(user.Email) != null)
                return BadRequest(new { Message = "Email already exists" });
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
            await _userService.CreateUserAsync(user);
            return Ok(new { Message = "User registered successfully" });
        }
    }

    public class LoginRequest
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    public class RegisterRequest
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string Email { get; set; }
    }
}