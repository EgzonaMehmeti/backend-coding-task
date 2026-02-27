using Claims.DTOs;
using Claims.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Controllers;

[ApiController]
[Route("v1/claims/cover")]
public class CoversController : ControllerBase
{
    private readonly ICoverService _service;

    public CoversController(ICoverService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CoverDto>>> Get()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CoverDto>> Get(string id)
    {
        var cover = await _service.GetByIdAsync(id);
        if (cover == null)
            return NotFound();

        return Ok(cover);
    }

    [HttpPost]
    public async Task<ActionResult<CoverDto>> Create(CreateCoverDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }

}
