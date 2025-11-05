using EnvironmentDriftChecker.Domain.Entities;

namespace EnvironmentDriftChecker.Data.Repositories.Interfaces
{
    public interface IConfigurationItemRepository
    {
        Task<IEnumerable<ConfigurationItem>> GetAllAsync();
        Task<IEnumerable<ConfigurationItem>> GetByEnvironmentIdAsync(int environmentId);
        Task<ConfigurationItem?> GetByIdAsync(int id);
        Task<ConfigurationItem?> GetByKeyAsync(int environmentId, string key);
        Task<ConfigurationItem> AddAsync(ConfigurationItem item);
        Task UpdateAsync(ConfigurationItem item);
        Task DeleteAsync(int id);
        Task<bool> KeyExistsAsync(int environmentId, string key);
    }
}
