using Microsoft.EntityFrameworkCore;
using EnvironmentDriftChecker.Data.Context;
using EnvironmentDriftChecker.Data.Repositories.Interfaces;
using EnvironmentDriftChecker.Domain.Entities;

namespace EnvironmentDriftChecker.Data.Repositories
{
    public class DriftRecordRepository : IDriftRecordRepository
    {
        private readonly DriftCheckerDbContext _context;

        public DriftRecordRepository(DriftCheckerDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DriftRecord>> GetAllAsync()
        {
            return await _context.DriftRecords
                .Include(d => d.SourceEnvironment)
                .Include(d => d.TargetEnvironment)
                .Include(d => d.DriftDetails)
                .OrderByDescending(d => d.ComparisonDate)
                .ToListAsync();
        }

        public async Task<DriftRecord?> GetByIdAsync(int id)
        {
            return await _context.DriftRecords
                .Include(d => d.SourceEnvironment)
                .Include(d => d.TargetEnvironment)
                .Include(d => d.DriftDetails)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<IEnumerable<DriftRecord>> GetByEnvironmentAsync(int environmentId)
        {
            return await _context.DriftRecords
                .Include(d => d.SourceEnvironment)
                .Include(d => d.TargetEnvironment)
                .Include(d => d.DriftDetails)
                .Where(d => d.SourceEnvironmentId == environmentId || d.TargetEnvironmentId == environmentId)
                .OrderByDescending(d => d.ComparisonDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<DriftRecord>> GetRecentAsync(int count = 10)
        {
            return await _context.DriftRecords
                .Include(d => d.SourceEnvironment)
                .Include(d => d.TargetEnvironment)
                .Include(d => d.DriftDetails)
                .OrderByDescending(d => d.ComparisonDate)
                .Take(count)
                .ToListAsync();
        }

        public async Task<DriftRecord> AddAsync(DriftRecord record)
        {
            record.ComparisonDate = DateTime.UtcNow;
            _context.DriftRecords.Add(record);
            await _context.SaveChangesAsync();
            return record;
        }

        public async Task DeleteAsync(int id)
        {
            var record = await _context.DriftRecords.FindAsync(id);
            if (record != null)
            {
                _context.DriftRecords.Remove(record);
                await _context.SaveChangesAsync();
            }
        }
    }
}