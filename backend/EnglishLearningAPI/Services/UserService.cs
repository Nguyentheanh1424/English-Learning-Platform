using MongoDB.Driver;
using BCrypt.Net;

using EnglishLearningAPI.Models;
using EnglishLearningAPI.Data;

using System.Threading.Tasks;
using System.Collections.Generic;

namespace EnglishLearningAPI.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;

        public UserService(MongoDbContext dbContext)
        {
            _users = dbContext.Users;
        }

        // Lấy danh sách tất cả người dùng
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _users.Find(user => true).ToListAsync();
        }

        // Lấy người dùng theo email
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _users.Find(user => user.Email == email).FirstOrDefaultAsync();
        }

        // Lấy người dùng theo id
        public async Task<User?> GetUserByIdAsync(string id)
        {
            return await _users.Find(user => user.Id == id).FirstOrDefaultAsync();
        }

        // Thêm người dùng với Hash mật khẩu
        public async Task<bool> CreateUserAsync(User user)
        {
            // Kiểm tra xem email đã tồn tại chưa
            var existingUser = await GetUserByEmailAsync(user.Email);
            if (existingUser != null) return false; // Email đã tồn tại

            // Hash mật khẩu trước khi lưu
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);

            await _users.InsertOneAsync(user);
            return true;
        }

        // Cập nhật người dùng (không thay đổi mật khẩu nếu không cần)
        public async Task<bool> UpdateUserAsync(string id, User updatedUser)
        {
            var existingUser = await GetUserByIdAsync(id);
            if (existingUser == null) return false;

            // Nếu mật khẩu mới không được cung cấp, giữ nguyên mật khẩu cũ
            if (string.IsNullOrWhiteSpace(updatedUser.PasswordHash))
            {
                updatedUser.PasswordHash = existingUser.PasswordHash;
            }
            else
            {
                // Nếu có mật khẩu mới, hash trước khi lưu
                updatedUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(updatedUser.PasswordHash);
            }

            updatedUser.UpdatedDate = DateTime.UtcNow; // Cập nhật thời gian sửa đổi

            var result = await _users.ReplaceOneAsync(user => user.Id == id, updatedUser);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        // Xóa người dùng
        public async Task<bool> DeleteUserAsync(string id)
        {
            var result = await _users.DeleteOneAsync(user => user.Id == id);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }

        // Kiểm tra mật khẩu khi đăng nhập
        public async Task<User?> AuthenticateUserAsync(string email, string password)
        {
            var user = await GetUserByEmailAsync(email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return null; // Sai mật khẩu hoặc không tìm thấy user
            }
            return user;
        }
    }
}
