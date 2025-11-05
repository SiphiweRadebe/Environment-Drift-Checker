using EnvironmentDriftChecker.Domain.Entities;

namespace EnvironmentDriftChecker.Data.Repositories.Interfaces
{
    public interface IEnvironmentRepository
    {
        Task<IEnumerable<Domain.Entities.Environment>> GetAllAsync();
        Task<Domain.Entities.Environment?> GetByIdAsync(int id);
        Task<Domain.Entities.Environment?> GetByNameAsync(string name);
        Task<Domain.Entities.Environment> AddAsync(Domain.Entities.Environment environment);
        Task UpdateAsync(Domain.Entities.Environment environment);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> NameExistsAsync(string name);
    }
}
