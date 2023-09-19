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
        /// 通过用户唯一标识获取用户权限
        /// </summary>
        /// <param name="userId">用户唯一标识</param>
        /// <returns>用户权限</returns>
        public async Task<List<Permission>> GetPermissionsByUserIdAsync(long userId)
        {
            var userPermissions = await _dbContext.Permissions
                .Where(p => p.RolePermissions.Any(rp => rp.Role.UserRoles.Any(ur => ur.UserId == userId)))
                .ToListAsync() ?? new List<Permission>();

            return userPermissions;
        }
    }
}
