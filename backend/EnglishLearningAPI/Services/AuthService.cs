using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using BCrypt.Net;
using EnglishLearningAPI.Models;
using System.Text.RegularExpressions;

namespace EnglishLearningAPI.Services
{
    public class AuthService
    {
        private readonly UserService _userService;
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _expiryMinutes;

        public AuthService(UserService userService, IOptions<JwtSettings> jwtSettings)
        {
            _userService = userService;
            _secretKey = jwtSettings.Value.SecretKey ?? throw new ArgumentNullException(nameof(jwtSettings.Value.SecretKey));
            _issuer = jwtSettings.Value.Issuer ?? throw new ArgumentNullException(nameof(jwtSettings.Value.Issuer));
            _audience = jwtSettings.Value.Audience ?? throw new ArgumentNullException(nameof(jwtSettings.Value.Audience));
            _expiryMinutes = jwtSettings.Value.ExpiryMinutes > 0 ? jwtSettings.Value.ExpiryMinutes : 60;
        }

        // 🔹 Đăng ký user mới (Hash mật khẩu trước khi lưu)
        public async Task<bool> RegisterUserAsync(User user)
        {
            if (string.IsNullOrWhiteSpace(user.Email) 
                || string.IsNullOrWhiteSpace(user.PasswordHash)
                || string.IsNullOrWhiteSpace(user.Username))
                throw new ArgumentException("Thông tin không được để trống.");

            // Chuẩn hóa email về chữ thường
            user.Email = user.Email.ToLower();

            // Kiểm tra định dạng email hợp lệ
            if (!Regex.IsMatch(user.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                throw new ArgumentException("Email không hợp lệ.");

            var existingUser = await _userService.GetUserByEmailAsync(user.Email);
            if (existingUser != null)
                return false; // User đã tồn tại

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
            await _userService.CreateUserAsync(user);
            return true;
        }

        // 🔹 Xác thực user và tạo JWT Token
        public async Task<string?> Authenticate(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return null;

            var user = await _userService.GetUserByEmailAsync(email.ToLower());
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return null;

            return GenerateJwtToken(user);
        }

        // 🔹 Tạo JWT Token
        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secretKey);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = _issuer,
                Audience = _audience,
                Expires = DateTime.UtcNow.AddMinutes(_expiryMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
