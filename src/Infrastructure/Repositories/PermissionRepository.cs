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
            var userRoles = await _dbContext.UserRoles
                .Where(x => x.UserId == userId)
                .ToListAsync();
            if (!userRoles.Any()) return new List<Permission>();

            var rolePermissions = await _dbContext.RolePermissions
                .Where(x => userRoles.Select(s => s.RoleId).Contains(x.RoleId))
                .ToListAsync();
            if (!rolePermissions.Any()) return new List<Permission>();

            var userPermissions = await _dbContext.Permissions
                .Where(x => rolePermissions.Select(s => s.PermissionId).Contains(x.Id))
                .ToListAsync() ?? new List<Permission>();

            return userPermissions;
        }
    }
}
