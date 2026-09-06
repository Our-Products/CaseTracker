using CaseTrackerApplication.Interfaces.Repositories;
using CaseTrackerInfrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CaseTrackerInfrastructure.Repositories
{
    public abstract class Repository<T> : IRepository<T>
        where T : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        protected Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> FindAsync(
            Expression<Func<T, bool>> predicate)
        {
            return await _dbSet
                .Where(predicate)
                .ToListAsync();
        }

        public virtual async Task<bool> ExistsAsync(object id)
        {
            return await _dbSet.FindAsync(id) != null;
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);

            return entity;
        }

        public virtual async Task AddRangeAsync(
            IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public virtual Task<T> UpdateAsync(T entity)
        {
            _dbSet.Update(entity);

            return Task.FromResult(entity);
        }

        public virtual async Task DeleteAsync(object id)
        {
            var entity = await GetByIdAsync(id);

            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }

        public virtual Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);

            return Task.CompletedTask;
        }

        public virtual Task DeleteRangeAsync(
            IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);

            return Task.CompletedTask;
        }

        public virtual async Task<int> CountAsync()
        {
            return await _dbSet.CountAsync();
        }
    }
}