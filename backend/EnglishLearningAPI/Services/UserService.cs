using MongoDB.Driver;

using EnglishLearningAPI.Models;
using EnglishLearningAPI.Data;

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

        // Lấy người dùng theo ID
        public async Task<User?> GetUserByIdAsync(string id)
        {
            return await _users.Find(user => user.Id == id).FirstOrDefaultAsync();
        }

        // Thêm người dùng
        public async Task CreateUserAsync(User user)
        {
            await _users.InsertOneAsync(user);
        }

        // Cập nhật người dùng
        public async Task<bool> UpdateUserAsync(string id, User updatedUser)
        {
            var result = await _users.ReplaceOneAsync(user => user.Id == id, updatedUser);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        // Xóa người dùng
        public async Task<bool> DeleteUserAsync(string id)
        {
            var result = await _users.DeleteOneAsync(user => user.Id == id);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }
    }
}