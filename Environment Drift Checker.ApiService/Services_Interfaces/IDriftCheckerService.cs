using EnvironmentDriftChecker.ApiService.DTOs;

namespace EnvironmentDriftChecker.ApiService.Services.Interfaces
{
    public interface IDriftCheckerService
    {
        Task<ComparisonResultDto> CompareEnvironmentsAsync(int sourceEnvId, int targetEnvId);
        Task<IEnumerable<ComparisonResultDto>> GetComparisonHistoryAsync();
        Task<IEnumerable<ComparisonResultDto>> GetEnvironmentHistoryAsync(int environmentId);
        Task<ComparisonResultDto?> GetComparisonByIdAsync(int id);
    }
}