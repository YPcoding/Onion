using Domain.Entities;
using Domain.Repositories;

namespace Infrastructure.Repositories
{
    /// <summary>
    /// 角色仓储
    /// </summary>
    public class RoleRepository : IRoleRepository
    {
        private readonly IApplicationDbContext _dbContext;

        public RoleRepository(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Role?> FindByIdAsync(long roleId)
        {
            return await _dbContext.Roles.FirstOrDefaultAsync(x => x.Id == roleId);
        }

        public async Task<Role?> FindByNameAsync(string roleName)
        {
            return await _dbContext.Roles.FirstOrDefaultAsync(x => x.RoleName == roleName);
        }

        public async Task<IEnumerable<Role>?> GetAllAsync()
        {
            return await _dbContext.Roles.ToListAsync();
        }

        public async Task<IEnumerable<Role>?> GetAllAsync(Expression<Func<Role, bool>> condition)
        {
            return await _dbContext.Roles.Where(condition).ToListAsync();
        }

        public async Task<IEnumerable<Role>?> GetUserRolesAsync(Expression<Func<Role, bool>> condition)
        {
            return await _dbContext.Roles
                .Include(ur => ur.UserRoles)
                .ThenInclude(u => u.User)
                .Include(rp => rp.RoleMenus)
                .ThenInclude(r => r.Menu)
                .Where(condition).ToListAsync();
        }
    }
}
