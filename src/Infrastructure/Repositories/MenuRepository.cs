using Domain.Entities.Identity;
using Domain.Enums;
using Domain.Repositories;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
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
        IQueryable<Menu> query = _dbContext.Menus
            .Include(m => m.Meta)
            .Include(rm => rm.RoleMenus)
                .ThenInclude(r => r.Role)
                    .ThenInclude(ru => ru.UserRoles);

        if (condition != null)
        {
            query = query.Where(condition);
        }

        return await query.ToListAsync(cancellationToken);
    }
    
    /// <summary>
    /// 通过用户唯一标识获取菜单
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<List<Menu>> GetAllByUserIdAsync(long userId)
    {
        var userMenus = (await GetAllAsync(menu => menu.RoleMenus
                .Any(rm => rm.Role.UserRoles.Any(ur => ur.UserId == userId)) && menu.Meta.Hidden != true))
                .ToList();

        return userMenus;
    }
}
