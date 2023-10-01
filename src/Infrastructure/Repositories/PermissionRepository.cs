using Domain.Entities;
using Domain.Repositories;

namespace Infrastructure.Repositories
{
    /// <summary>
    /// 权限仓储
    /// </summary>
    public class PermissionRepository : IPermissionRepository
    {
        private readonly IApplicationDbContext _dbContext;

        public PermissionRepository(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// 通过条件获取所有权限
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>权限</returns>
        public async Task<List<Permission>> GetAllAsync(Expression<Func<Permission, bool>>? condition, CancellationToken cancellationToken = default)
        {
            IQueryable<Permission> query = _dbContext.Permissions;

            if (condition != null)
            {
                query = query.Where(condition);
            }

            return await query.ToListAsync();
        }

        /// <summary>
        /// 通过角色唯一标识获取角色权限
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns>角色权限</returns>
        public async Task<List<Permission>> GetPermissionsByRoleIdAsync(long roleId)
        {
            var rolePermissions = await _dbContext.Permissions
                .Where(p => p.RolePermissions.Any(rp => rp.Role.UserRoles.Any(ur => ur.RoleId == roleId)))
                .ToListAsync() ?? new List<Permission>();

            return rolePermissions;
        }

        /// <summary>
        /// 通过用户唯一标识获取用户权限
        /// </summary>
        /// <param name="userId">用户唯一标识</param>
        /// <returns>用户权限</returns>
        public async Task<List<Permission>> GetPermissionsByUserIdAsync(long userId)
        {
            var userPermissions = await _dbContext.Permissions
                .Where(p => p.RolePermissions.Any(rp => rp.Role.UserRoles.Any(ur => ur.UserId == userId)) && p.Enabled == true && p.Hidden == false)
                .OrderBy(o => o.Sort)
                .ToListAsync();

            return userPermissions ?? new List<Permission>();
        }
    }
}
