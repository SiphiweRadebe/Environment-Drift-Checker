using Microsoft.EntityFrameworkCore;
using EnvironmentDriftChecker.Data.Context;
using EnvironmentDriftChecker.Data.Repositories.Interfaces;
using EnvironmentDriftChecker.Domain.Entities;

namespace EnvironmentDriftChecker.Data.Repositories
{
    public class ConfigurationItemRepository : IConfigurationItemRepository
    {
        private readonly DriftCheckerDbContext _context;

        public ConfigurationItemRepository(DriftCheckerDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ConfigurationItem>> GetAllAsync()
        {
            return await _context.ConfigurationItems
                .Include(c => c.Environment)
                .ToListAsync();
        }

        public async Task<IEnumerable<ConfigurationItem>> GetByEnvironmentIdAsync(int environmentId)
        {
            return await _context.ConfigurationItems
                .Where(c => c.EnvironmentId == environmentId)
                .OrderBy(c => c.Key)
                .ToListAsync();
        }

        public async Task<ConfigurationItem?> GetByIdAsync(int id)
        {
            return await _context.ConfigurationItems
                .Include(c => c.Environment)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<ConfigurationItem?> GetByKeyAsync(int environmentId, string key)
        {
            return await _context.ConfigurationItems
                .FirstOrDefaultAsync(c => c.EnvironmentId == environmentId && c.Key == key);
        }

        public async Task<ConfigurationItem> AddAsync(ConfigurationItem item)
        {
            item.LastUpdated = DateTime.UtcNow;
            _context.ConfigurationItems.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task UpdateAsync(ConfigurationItem item)
        {
            item.LastUpdated = DateTime.UtcNow;
            _context.ConfigurationItems.Update(item);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var item = await _context.ConfigurationItems.FindAsync(id);
            if (item != null)
            {
                _context.ConfigurationItems.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> KeyExistsAsync(int environmentId, string key)
        {
            return await _context.ConfigurationItems
                .AnyAsync(c => c.EnvironmentId == environmentId && c.Key == key);
        }
    }
}