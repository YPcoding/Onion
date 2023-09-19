using Domain.Entities;

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
}
