using EnvironmentDriftChecker.ApiService.DTOs;

namespace EnvironmentDriftChecker.ApiService.Services.Interfaces
{
    public interface IEnvironmentService
    {
        Task<IEnumerable<EnvironmentDto>> GetAllEnvironmentsAsync();
        Task<EnvironmentDto?> GetEnvironmentByIdAsync(int id);
        Task<EnvironmentDto> CreateEnvironmentAsync(CreateEnvironmentDto dto);
        Task<EnvironmentDto> UpdateEnvironmentAsync(int id, UpdateEnvironmentDto dto);
        Task DeleteEnvironmentAsync(int id);
        Task<IEnumerable<ConfigurationItemDto>> GetEnvironmentConfigurationsAsync(int environmentId);
    }
}