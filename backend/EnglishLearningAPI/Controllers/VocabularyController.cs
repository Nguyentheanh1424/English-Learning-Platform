using Microsoft.AspNetCore.Mvc;

using EnglishLearningAPI.Services;
using EnglishLearningAPI.Models;


[Route("api/vocabulary")]
[ApiController]
public class VocabularyController : ControllerBase
{
    private readonly VocabularyService _service;

    public VocabularyController(VocabularyService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id) => Ok(await _service.GetByIdAsync(id));

    [HttpPost]
    public async Task<IActionResult> Create(Vocabulary vocab)
    {
        await _service.CreateAsync(vocab);
        return CreatedAtAction(nameof(GetById), new { id = vocab.Id }, vocab);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, Vocabulary vocab)
    {
        await _service.UpdateAsync(id, vocab);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
