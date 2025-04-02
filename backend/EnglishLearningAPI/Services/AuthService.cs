using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using EnglishLearningAPI.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using BC = BCrypt.Net.BCrypt;

namespace EnglishLearningAPI.Services
{
    public class AuthService
    {
        private readonly UserService _userService;
        private readonly JwtSettings _jwtSettings;

        public AuthService(UserService userService, IOptions<JwtSettings> jwtSettings)
        {
            _userService = userService;
            _jwtSettings = jwtSettings.Value ?? throw new ArgumentNullException(nameof(jwtSettings.Value));
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

            user.PasswordHash = BC.HashPassword(user.PasswordHash);
            await _userService.CreateUserAsync(user);
            return true;
        }

        // 🔹 Xác thực user và tạo JWT Token
        public async Task<string?> Authenticate(string email, string password)
        {
            email = email.ToLower().Trim();
            password = password.Trim();

            var user = await _userService.GetUserByEmailAsync(email);
            if (user == null)
            {
                Console.WriteLine($"❌ Không tìm thấy user với email: {email}");
                return null;
            }

            Console.WriteLine($"🔹 Mật khẩu nhập vào: '{password}' (Length: {password.Length})");
            Console.WriteLine($"🔹 Mật khẩu hash trong DB: '{user.PasswordHash}' (Length: {user.PasswordHash.Length})");

            if (!BC.Verify(password, user.PasswordHash))
            {
                Console.WriteLine("❌ Sai mật khẩu!");
                return null;
            }

            Console.WriteLine($"✅ Xác thực thành công cho user {user.Username}");
            return GenerateJwtToken(user);
        }


        // 🔹 Tạo JWT Token
        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

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
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
