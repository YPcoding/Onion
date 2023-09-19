using Domain.Entities;
using System.Linq.Expressions;

namespace Domain.Repositories
{
    /// <summary>
    /// 角色仓储
    /// </summary>
    public interface IRoleRepository : IScopedDependency
    {
        /// <summary>
        /// 根据唯一标识获取角色
        /// </summary>
        /// <param name="roleId">角色唯一标识</param>
        /// <returns></returns>
        Task<Role?> FindByIdAsync(long roleId);

        /// <summary>
        /// 获取所有角色
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Role>?> GetAllAsync();

        /// <summary>
        /// 获取所有角色
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Role>?> GetAllAsync(Expression<Func<Role, bool>> condition);

        /// <summary>
        /// 获取用户所有角色
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Role>?> GetUserRolesAsync(Expression<Func<Role, bool>> condition);

        /// <summary>
        /// 根据角色名称获取角色
        /// </summary>
        /// <param name="roleName">角色名称</param>
        /// <returns></returns>
        Task<Role?> FindByNameAsync(string roleName);
    }
}
