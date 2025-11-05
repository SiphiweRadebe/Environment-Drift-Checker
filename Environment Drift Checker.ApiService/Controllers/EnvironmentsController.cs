using Microsoft.AspNetCore.Mvc;
using EnvironmentDriftChecker.ApiService.DTOs;
using EnvironmentDriftChecker.ApiService.Services.Interfaces;

namespace EnvironmentDriftChecker.ApiService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnvironmentsController : ControllerBase
    {
        private readonly IEnvironmentService _environmentService;
        private readonly ILogger<EnvironmentsController> _logger;

        public EnvironmentsController(IEnvironmentService environmentService, ILogger<EnvironmentsController> logger)
        {
            _environmentService = environmentService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EnvironmentDto>>> GetAll()
        {
            var environments = await _environmentService.GetAllEnvironmentsAsync();
            return Ok(environments);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EnvironmentDto>> GetById(int id)
        {
            var environment = await _environmentService.GetEnvironmentByIdAsync(id);
            if (environment == null)
                return NotFound(new { message = $"Environment with ID {id} not found" });

            return Ok(environment);
        }

        [HttpPost]
        public async Task<ActionResult<EnvironmentDto>> Create([FromBody] CreateEnvironmentDto dto)
        {
            try
            {
                var created = await _environmentService.CreateEnvironmentAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<EnvironmentDto>> Update(int id, [FromBody] UpdateEnvironmentDto dto)
        {
            try
            {
                var updated = await _environmentService.UpdateEnvironmentAsync(id, dto);
                return Ok(updated);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _environmentService.DeleteEnvironmentAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("{id}/configurations")]
        public async Task<ActionResult<IEnumerable<ConfigurationItemDto>>> GetConfigurations(int id)
        {
            var configs = await _environmentService.GetEnvironmentConfigurationsAsync(id);
            return Ok(configs);
        }
    }
}