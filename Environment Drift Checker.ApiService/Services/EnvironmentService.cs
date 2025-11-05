using EnvironmentDriftChecker.ApiService.DTOs;
using EnvironmentDriftChecker.ApiService.Services.Interfaces;
using EnvironmentDriftChecker.Data.Repositories.Interfaces;
using EnvironmentDriftChecker.Domain.Entities;

namespace EnvironmentDriftChecker.ApiService.Services
{
    public class EnvironmentService : IEnvironmentService
    {
        private readonly IEnvironmentRepository _environmentRepo;
        private readonly IConfigurationItemRepository _configRepo;
        private readonly ILogger<EnvironmentService> _logger;

        public EnvironmentService(
            IEnvironmentRepository environmentRepo,
            IConfigurationItemRepository configRepo,
            ILogger<EnvironmentService> logger)
        {
            _environmentRepo = environmentRepo;
            _configRepo = configRepo;
            _logger = logger;
        }

        public async Task<IEnumerable<EnvironmentDto>> GetAllEnvironmentsAsync()
        {
            var environments = await _environmentRepo.GetAllAsync();
            return environments.Select(e => new EnvironmentDto
            {
                Id = e.Id,
                Name = e.Name,
                Description = e.Description,
                CreatedAt = e.CreatedAt,
                IsActive = e.IsActive,
                ConfigurationCount = e.ConfigurationItems.Count
            });
        }

        public async Task<EnvironmentDto?> GetEnvironmentByIdAsync(int id)
        {
            var environment = await _environmentRepo.GetByIdAsync(id);
            if (environment == null) return null;

            return new EnvironmentDto
            {
                Id = environment.Id,
                Name = environment.Name,
                Description = environment.Description,
                CreatedAt = environment.CreatedAt,
                IsActive = environment.IsActive,
                ConfigurationCount = environment.ConfigurationItems.Count
            };
        }

        public async Task<EnvironmentDto> CreateEnvironmentAsync(CreateEnvironmentDto dto)
        {
            // Validation: Check if name already exists
            if (await _environmentRepo.NameExistsAsync(dto.Name))
            {
                throw new InvalidOperationException($"Environment with name '{dto.Name}' already exists.");
            }

            var environment = new Domain.Entities.Environment
            {
                Name = dto.Name,
                Description = dto.Description,
                IsActive = true
            };

            var created = await _environmentRepo.AddAsync(environment);
            _logger.LogInformation("Created new environment: {Name}", created.Name);

            return new EnvironmentDto
            {
                Id = created.Id,
                Name = created.Name,
                Description = created.Description,
                CreatedAt = created.CreatedAt,
                IsActive = created.IsActive,
                ConfigurationCount = 0
            };
        }

        public async Task<EnvironmentDto> UpdateEnvironmentAsync(int id, UpdateEnvironmentDto dto)
        {
            var environment = await _environmentRepo.GetByIdAsync(id);
            if (environment == null)
            {
                throw new KeyNotFoundException($"Environment with ID {id} not found.");
            }

            // Check if new name conflicts with existing
            if (environment.Name != dto.Name && await _environmentRepo.NameExistsAsync(dto.Name))
            {
                throw new InvalidOperationException($"Environment with name '{dto.Name}' already exists.");
            }

            environment.Name = dto.Name;
            environment.Description = dto.Description;
            environment.IsActive = dto.IsActive;

            await _environmentRepo.UpdateAsync(environment);
            _logger.LogInformation("Updated environment: {Name}", environment.Name);

            return new EnvironmentDto
            {
                Id = environment.Id,
                Name = environment.Name,
                Description = environment.Description,
                CreatedAt = environment.CreatedAt,
                IsActive = environment.IsActive,
                ConfigurationCount = environment.ConfigurationItems.Count
            };
        }

        public async Task DeleteEnvironmentAsync(int id)
        {
            if (!await _environmentRepo.ExistsAsync(id))
            {
                throw new KeyNotFoundException($"Environment with ID {id} not found.");
            }

            await _environmentRepo.DeleteAsync(id);
            _logger.LogInformation("Deleted environment with ID: {Id}", id);
        }

        public async Task<IEnumerable<ConfigurationItemDto>> GetEnvironmentConfigurationsAsync(int environmentId)
        {
            var configs = await _configRepo.GetByEnvironmentIdAsync(environmentId);
            return configs.Select(c => new ConfigurationItemDto
            {
                Id = c.Id,
                EnvironmentId = c.EnvironmentId,
                Key = c.Key,
                Value = c.IsSensitive ? "***HIDDEN***" : c.Value,
                Category = c.Category,
                LastUpdated = c.LastUpdated,
                IsSensitive = c.IsSensitive
            });
        }
    }
}
