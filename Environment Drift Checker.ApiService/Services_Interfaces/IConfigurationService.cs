using EnvironmentDriftChecker.ApiService.DTOs;

namespace EnvironmentDriftChecker.ApiService.Services.Interfaces
{
    public interface IConfigurationService
    {
        Task<ConfigurationItemDto?> GetConfigurationByIdAsync(int id);
        Task<ConfigurationItemDto> CreateConfigurationAsync(CreateConfigurationItemDto dto);
        Task<ConfigurationItemDto> UpdateConfigurationAsync(int id, UpdateConfigurationItemDto dto);
        Task DeleteConfigurationAsync(int id);
    }
}
