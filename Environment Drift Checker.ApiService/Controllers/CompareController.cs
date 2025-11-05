using Microsoft.AspNetCore.Mvc;
using EnvironmentDriftChecker.ApiService.DTOs;
using EnvironmentDriftChecker.ApiService.Services.Interfaces;

namespace EnvironmentDriftChecker.ApiService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompareController : ControllerBase
    {
        private readonly IDriftCheckerService _driftService;
        private readonly ILogger<CompareController> _logger;

        public CompareController(IDriftCheckerService driftService, ILogger<CompareController> logger)
        {
            _driftService = driftService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ComparisonResultDto>> Compare(
            [FromQuery] int sourceEnvId,
            [FromQuery] int targetEnvId)
        {
            try
            {
                var result = await _driftService.CompareEnvironmentsAsync(sourceEnvId, targetEnvId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("history")]
        public async Task<ActionResult<IEnumerable<ComparisonResultDto>>> GetHistory()
        {
            var history = await _driftService.GetComparisonHistoryAsync();
            return Ok(history);
        }

        [HttpGet("history/{id}")]
        public async Task<ActionResult<ComparisonResultDto>> GetComparisonById(int id)
        {
            var comparison = await _driftService.GetComparisonByIdAsync(id);
            if (comparison == null)
                return NotFound(new { message = $"Comparison with ID {id} not found" });

            return Ok(comparison);
        }

        [HttpGet("environment/{environmentId}")]
        public async Task<ActionResult<IEnumerable<ComparisonResultDto>>> GetEnvironmentHistory(int environmentId)
        {
            var history = await _driftService.GetEnvironmentHistoryAsync(environmentId);
            return Ok(history);
        }
    }
}
