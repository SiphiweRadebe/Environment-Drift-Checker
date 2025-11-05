using Microsoft.AspNetCore.Mvc;
using EnvironmentDriftChecker.ApiService.DTOs;
using EnvironmentDriftChecker.ApiService.Services.Interfaces;

namespace EnvironmentDriftChecker.ApiService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConfigurationsController : ControllerBase
    {
        private readonly IConfigurationService _configService;
        private readonly ILogger<ConfigurationsController> _logger;

        public ConfigurationsController(IConfigurationService configService, ILogger<ConfigurationsController> logger)
        {
            _configService = configService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ConfigurationItemDto>> GetById(int id)
        {
            var config = await _configService.GetConfigurationByIdAsync(id);
            if (config == null)
                return NotFound(new { message = $"Configuration with ID {id} not found" });

            return Ok(config);
        }

        [HttpPost]
        public async Task<ActionResult<ConfigurationItemDto>> Create([FromBody] CreateConfigurationItemDto dto)
        {
            try
            {
                var created = await _configService.CreateConfigurationAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
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

        [HttpPut("{id}")]
        public async Task<ActionResult<ConfigurationItemDto>> Update(int id, [FromBody] UpdateConfigurationItemDto dto)
        {
            try
            {
                var updated = await _configService.UpdateConfigurationAsync(id, dto);
                return Ok(updated);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _configService.DeleteConfigurationAsync(id);
            return NoContent();
        }
    }
}
