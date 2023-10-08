using Domain.Entities.Identity;
using Domain.Repositories;
using System.Linq.Dynamic.Core;

namespace Infrastructure.Repositories;

/// <summary>
/// 菜单仓储
/// </summary>
public class MenuRepository : IMenuRepository
{
    private readonly IApplicationDbContext _dbContext;

    public MenuRepository(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// 获取所有菜单
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<List<Menu>> GetAllAsync(Expression<Func<Menu, bool>>? condition = null, CancellationToken cancellationToken = default)
    {
        IQueryable<Menu> query = _dbContext.Menus.Include(m => m.Meta);

        if (condition != null)
        {
            query = query.Where(condition);
        }

        return await query.ToListAsync(cancellationToken);
    }
}
