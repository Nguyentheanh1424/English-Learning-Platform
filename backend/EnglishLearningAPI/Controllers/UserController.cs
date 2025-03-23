using Microsoft.AspNetCore.Mvc;

using EnglishLearningAPI.Models;
using EnglishLearningAPI.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;

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
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        // Lấy người dùng theo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        // Thêm người dùng
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            await _userService.CreateUserAsync(user);
            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        }

        // Cập nhật người dùng
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] User user)
        {
            var existingUser = await _userService.GetUserByIdAsync(id);
            if (existingUser == null)
            {
                return NotFound();
            }

            user.Id = id;
            var result = await _userService.UpdateUserAsync(id, user);
            if (!result)
            {
                return StatusCode(500);
            }
            return Ok(user);
        }

        // Xóa người dùng
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id){
            var existingUser = await _userService.GetUserByIdAsync(id);
            if (existingUser == null)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}