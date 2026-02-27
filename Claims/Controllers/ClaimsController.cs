using Claims.DTOs;
using Claims.Models;
using Claims.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Controllers
{
    [ApiController]
    [Route("v1/claims")]
    public class ClaimsController : ControllerBase
    {
        private readonly IClaimService _service;
        public ClaimsController(IClaimService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClaimDto>>> Get()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Claim>> Get(string id)
        {
            var claim = await _service.GetByIdAsync(id);
            if (claim == null) return NotFound();
            return Ok(claim);
        }

        [HttpPost]
        public async Task<ActionResult<ClaimDto>> Create(CreateClaimDto claim)
        {
            return Ok(await _service.CreateAsync(claim));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
