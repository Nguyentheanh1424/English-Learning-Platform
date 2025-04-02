using Microsoft.AspNetCore.Mvc;

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

        public AuthController(AuthService authService, UserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

        // API Đăng ký
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (await _userService.GetUserByEmailAsync(request.Email) != null)
                return BadRequest(new { message = "Email đã tồn tại!" });

            var newUser = new User
            {
                Username = request.Username,
                Email = request.Email.ToLower().Trim(),
                PasswordHash = request.Password,
                Role = "Student",
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };


            await _userService.CreateUserAsync(newUser);
            return Ok(new { message = "Đăng ký thành công!" });
        }


        // API Đăng nhập
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var token = await _authService.Authenticate(request.Email, request.Password);
            if (token == null)
                return Unauthorized(new { message = "Sai email hoặc mật khẩu!" });

            return Ok(new { Token = token });
        }
    }
}