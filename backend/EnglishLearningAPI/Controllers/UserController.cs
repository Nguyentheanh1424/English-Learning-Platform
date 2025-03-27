using Microsoft.AspNetCore.Mvc;

using EnglishLearningAPI.Models;
using EnglishLearningAPI.Services;

namespace EnglishLearningAPI.Controllers
{
    [Route("api/users")]
    [ApiController]
    
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        // Lấy danh sách tất cả người dùng
        [HttpGet("allID")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        // Lấy người dùng theo ID
        [HttpGet("id/{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        // Lấy người dùng theo Email
        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var user = await _userService.GetUserByEmailAsync(email);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        // Thêm người dùng
        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            if (user == null || string.IsNullOrWhiteSpace(user.Email))
            {
                return BadRequest("Dữ liệu không hợp lệ.");
            }

            await _userService.CreateUserAsync(user);
            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        }

        // Cập nhật người dùng
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] User user)
        {
            var existingUser = await _userService.GetUserByIdAsync(id);
            if (existingUser == null)
            {
                return NotFound();
            }

            // Cập nhật thông tin hợp lệ
            existingUser.Username = user.Username ?? existingUser.Username;
            existingUser.Email = user.Email ?? existingUser.Email;
            existingUser.Role = user.Role ?? existingUser.Role;

            var result = await _userService.UpdateUserAsync(id, user);
            if (!result)
            {
                return StatusCode(500, "Cập nhật thất bại!");
            }
            return Ok(existingUser);
        }

        // Xóa người dùng
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var existingUser = await _userService.GetUserByIdAsync(id);
            if (existingUser == null)
            {
                return NotFound();
            }

            var result = await _userService.DeleteUserAsync(id);
            if (!result)
            {
                return StatusCode(500, "Xóa thất bại.");
            }

            return NoContent();
        }
    }
}