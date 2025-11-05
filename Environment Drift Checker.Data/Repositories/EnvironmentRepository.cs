using Microsoft.EntityFrameworkCore;
using EnvironmentDriftChecker.Data.Context;
using EnvironmentDriftChecker.Data.Repositories.Interfaces;
using EnvironmentDriftChecker.Domain.Entities;

namespace EnvironmentDriftChecker.Data.Repositories
{
    public class EnvironmentRepository : IEnvironmentRepository
    {
        private readonly DriftCheckerDbContext _context;

        public EnvironmentRepository(DriftCheckerDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Domain.Entities.Environment>> GetAllAsync()
        {
            return await _context.Environments
                .Include(e => e.ConfigurationItems)
                .OrderBy(e => e.Name)
                .ToListAsync();
        }

        public async Task<Domain.Entities.Environment?> GetByIdAsync(int id)
        {
            return await _context.Environments
                .Include(e => e.ConfigurationItems)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Domain.Entities.Environment?> GetByNameAsync(string name)
        {
            return await _context.Environments
                .Include(e => e.ConfigurationItems)
                .FirstOrDefaultAsync(e => e.Name == name);
        }

        public async Task<Domain.Entities.Environment> AddAsync(Domain.Entities.Environment environment)
        {
            environment.CreatedAt = DateTime.UtcNow;
            _context.Environments.Add(environment);
            await _context.SaveChangesAsync();
            return environment;
        }

        public async Task UpdateAsync(Domain.Entities.Environment environment)
        {
            _context.Environments.Update(environment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var environment = await _context.Environments.FindAsync(id);
            if (environment != null)
            {
                _context.Environments.Remove(environment);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Environments.AnyAsync(e => e.Id == id);
        }

        public async Task<bool> NameExistsAsync(string name)
        {
            return await _context.Environments.AnyAsync(e => e.Name == name);
        }
    }
}