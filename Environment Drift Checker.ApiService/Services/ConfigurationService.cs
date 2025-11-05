using EnvironmentDriftChecker.ApiService.DTOs;
using EnvironmentDriftChecker.ApiService.Services.Interfaces;
using EnvironmentDriftChecker.Data.Repositories.Interfaces;
using EnvironmentDriftChecker.Domain.Entities;

namespace EnvironmentDriftChecker.ApiService.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly IConfigurationItemRepository _configRepo;
        private readonly IEnvironmentRepository _environmentRepo;
        private readonly ILogger<ConfigurationService> _logger;

        public ConfigurationService(
            IConfigurationItemRepository configRepo,
            IEnvironmentRepository environmentRepo,
            ILogger<ConfigurationService> logger)
        {
            _configRepo = configRepo;
            _environmentRepo = environmentRepo;
            _logger = logger;
        }

        public async Task<ConfigurationItemDto?> GetConfigurationByIdAsync(int id)
        {
            var config = await _configRepo.GetByIdAsync(id);
            if (config == null) return null;

            return new ConfigurationItemDto
            {
                Id = config.Id,
                EnvironmentId = config.EnvironmentId,
                Key = config.Key,
                Value = config.IsSensitive ? "***HIDDEN***" : config.Value,
                Category = config.Category,
                LastUpdated = config.LastUpdated,
                IsSensitive = config.IsSensitive
            };
        }

        public async Task<ConfigurationItemDto> CreateConfigurationAsync(CreateConfigurationItemDto dto)
        {
            // Validate environment exists
            if (!await _environmentRepo.ExistsAsync(dto.EnvironmentId))
            {
                throw new KeyNotFoundException($"Environment with ID {dto.EnvironmentId} not found.");
            }

            // Validate key doesn't already exist for this environment
            if (await _configRepo.KeyExistsAsync(dto.EnvironmentId, dto.Key))
            {
                throw new InvalidOperationException($"Configuration key '{dto.Key}' already exists in this environment.");
            }

            var config = new ConfigurationItem
            {
                EnvironmentId = dto.EnvironmentId,
                Key = dto.Key,
                Value = dto.Value,
                Category = dto.Category,
                IsSensitive = dto.IsSensitive
            };

            var created = await _configRepo.AddAsync(config);
            _logger.LogInformation("Created configuration: {Key} in environment {EnvId}", created.Key, created.EnvironmentId);

            return new ConfigurationItemDto
            {
                Id = created.Id,
                EnvironmentId = created.EnvironmentId,
                Key = created.Key,
                Value = created.IsSensitive ? "***HIDDEN***" : created.Value,
                Category = created.Category,
                LastUpdated = created.LastUpdated,
                IsSensitive = created.IsSensitive
            };
        }

        public async Task<ConfigurationItemDto> UpdateConfigurationAsync(int id, UpdateConfigurationItemDto dto)
        {
            var config = await _configRepo.GetByIdAsync(id);
            if (config == null)
            {
                throw new KeyNotFoundException($"Configuration with ID {id} not found.");
            }

            config.Value = dto.Value;
            config.Category = dto.Category;
            config.IsSensitive = dto.IsSensitive;

            await _configRepo.UpdateAsync(config);
            _logger.LogInformation("Updated configuration: {Key}", config.Key);

            return new ConfigurationItemDto
            {
                Id = config.Id,
                EnvironmentId = config.EnvironmentId,
                Key = config.Key,
                Value = config.IsSensitive ? "***HIDDEN***" : config.Value,
                Category = config.Category,
                LastUpdated = config.LastUpdated,
                IsSensitive = config.IsSensitive
            };
        }

        public async Task DeleteConfigurationAsync(int id)
        {
            await _configRepo.DeleteAsync(id);
            _logger.LogInformation("Deleted configuration with ID: {Id}", id);
        }
    }
}