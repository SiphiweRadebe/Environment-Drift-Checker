using EnvironmentDriftChecker.Domain.Entities;

namespace EnvironmentDriftChecker.Data.Repositories.Interfaces
{
    public interface IDriftRecordRepository
    {
        Task<IEnumerable<DriftRecord>> GetAllAsync();
        Task<DriftRecord?> GetByIdAsync(int id);
        Task<IEnumerable<DriftRecord>> GetByEnvironmentAsync(int environmentId);
        Task<IEnumerable<DriftRecord>> GetRecentAsync(int count = 10);
        Task<DriftRecord> AddAsync(DriftRecord record);
        Task DeleteAsync(int id);
    }
}