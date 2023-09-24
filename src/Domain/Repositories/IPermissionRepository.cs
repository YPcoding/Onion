using Domain.Entities;
using System.Linq.Expressions;

namespace Domain.Repositories;

/// <summary>
/// 权限仓储
/// </summary>
public interface IPermissionRepository : IScopedDependency
{
    /// <summary>
    /// 通过用户唯一标识获取用户权限
    /// </summary>
    /// <param name="userId">用户唯一标识</param>
    /// <returns>用户权限</returns>
    Task<List<Permission>> GetPermissionsByUserIdAsync(long userId);

    /// <summary>
    /// 通过角色唯一标识获取角色权限
    /// </summary>
    /// <param name="roleId">角色唯一标识</param>
    /// <returns>用户权限</returns>
    Task<List<Permission>> GetPermissionsByRoleIdAsync(long roleId);

    /// <summary>
    /// 通过条件获取所有权限
    /// </summary>
    /// <param name="condition">条件</param>
    /// <param name="cancellationToken">取消标记</param>
    /// <returns>权限</returns>
    Task<List<Permission>> GetAllAsync(Expression<Func<Permission, bool>>? condition = null, CancellationToken cancellationToken = default);
}
