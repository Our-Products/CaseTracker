using CaseTrackerApplication.Interfaces.Repositories;
using CaseTrackerDomain.Models;
using CaseTrackerInfrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CaseTrackerInfrastructure.Repositories
{
    public class UserRoleRepository
        : Repository<UserRole>, IUserRoleRepository
    {
        public UserRoleRepository(ApplicationDbContext context)
            : base(context)
        {
        }

        public async Task<IEnumerable<string>> GetRoleNamesByUserIdAsync(Guid userId)
        {
            return await _dbSet
                .Where(x => x.UserId == userId)
                .Select(x => x.Role!.RoleName)
                .ToListAsync();
        }
        public async Task<IEnumerable<UserRole>> GetByUserIdAsync(
            Guid userId)
        {
            return await _dbSet
                .Where(x => x.UserId == userId)
                .Include(x => x.Role)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserRole>> GetByRoleIdAsync(
            string roleId)
        {
            return await _dbSet
                .Where(x => x.RoleId == roleId)
                .Include(x => x.User)
                .ToListAsync();
        }

        public async Task<UserRole?> GetByUserAndRoleAsync(
            Guid userId,
            string roleId)
        {
            return await _dbSet
                .Include(x => x.User)
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.RoleId == roleId);
        }

        public async Task<bool> IsUserAssignedToRoleAsync(
            Guid userId,
            string roleId)
        {
            return await _dbSet
                .AnyAsync(x =>
                    x.UserId == userId &&
                    x.RoleId == roleId);
        }

        public async Task<IEnumerable<UserRole>> GetAllActiveAsync()
        {
            return await _dbSet
          .Where(x =>
              x.User!.Status == "Active" &&
              x.Role!.Status == "Active")
          .Include(x => x.User)
          .Include(x => x.Role)
          .ToListAsync();
        }
    }
}