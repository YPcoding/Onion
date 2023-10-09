using System.Linq.Expressions;

namespace Domain.Repositories;

/// <summary>
/// 菜单仓储
/// </summary>
public interface IMenuRepository : IScopedDependency
{
    /// <summary>
    /// 通过条件获取所有菜单
    /// </summary>
    /// <param name="condition">条件</param>
    /// <param name="cancellationToken">取消标记</param>
    /// <returns>菜单数据</returns>
    Task<List<Menu>> GetAllAsync(Expression<Func<Menu, bool>>? condition = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 通过用户唯一标识获取所有菜单
    /// </summary>
    /// <param name="userId">用户唯一标识</param>
    /// <returns>菜单数据</returns>
    Task<List<Menu>> GetAllByUserIdAsync(long userId);
}
