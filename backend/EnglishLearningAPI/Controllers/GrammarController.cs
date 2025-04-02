using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

using EnglishLearningAPI.Models;
using EnglishLearningAPI.Services;

namespace EnglishLearningAPI.Controllers
{
    [Route("api/grammar")]
    [ApiController]
    public class GrammarController : ControllerBase
    {
        private readonly GrammarService _grammarService;

        public GrammarController(GrammarService grammarService)
        {
            _grammarService = grammarService;
        }

        // Lấy danh sách tất cả các quy tắc ngữ pháp
        [HttpGet]
        public async Task<ActionResult<List<Grammar>>> GetAllRules()
        {
            return Ok(await _grammarService.GetAllRulesAsync());
        }

        // Lấy một quy tắc ngữ pháp theo ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Grammar>> GetRuleById(string id)
        {
            var rule = await _grammarService.GetRuleByIdAsync(id);
            if (rule == null)
                return NotFound(new { message = "Không tìm thấy quy tắc ngữ pháp" });
            return Ok(rule);
        }

        // Thêm một quy tắc ngữ pháp mới
        [HttpPost]
        public async Task<IActionResult> AddRule([FromBody] Grammar rule)
        {
            if (rule == null || string.IsNullOrEmpty(rule.Rule) || string.IsNullOrEmpty(rule.Description))
                return BadRequest(new { message = "Dữ liệu không hợp lệ!" });

            await _grammarService.CreateRuleAsync(rule);
            return Ok(new { message = "Thêm quy tắc ngữ pháp thành công!" });
        }

        // Xóa một quy tắc ngữ pháp theo ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRule(string id)
        {
            var success = await _grammarService.DeleteRuleAsync(id);
            if (!success)
                return NotFound(new { message = "Không tìm thấy quy tắc ngữ pháp để xóa!" });

            return Ok(new { message = "Xóa quy tắc ngữ pháp thành công!" });
        }
    }
}
