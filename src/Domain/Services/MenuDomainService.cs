using Domain.Repositories;
using System.Linq.Expressions;

namespace Domain.Services;

/// <summary>
/// 菜单域服务
/// </summary>
public class MenuDomainService : IScopedDependency
{
    private readonly IMenuRepository _repository;

    public MenuDomainService(
        IMenuRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// 获取菜单树
    /// </summary>
    /// <returns></returns>
    public async Task<List<Menu>> GetMenuTreeAsync(Expression<Func<Menu, bool>>? condition = null, CancellationToken cancellationToken = default)
    {
        return TreeConverter.ConvertToTree(await _repository.GetAllAsync(condition, cancellationToken));
    }

    /// <summary>
    /// 获取仪表盘数据
    /// </summary>
    /// <returns></returns>
    public async Task<List<string>> GetDashboardGridsAsync() 
    {
        return await Task.Run(() => 
        {
            return new List<string>() 
            {
                "welcome",
                "ver",
                "time",
                "progress",
                "echarts",
                "about"
            };
        }) ;
    }


    /// <summary>
    /// 获取权限数据
    /// </summary>
    /// <returns></returns>
    public async Task<List<Menu>> GetPermissionsAsync(long userId)
    {
        return (await _repository.GetAllByUserIdAsync(userId))
               .Where(x => x.Meta.Type == MetaType.Api)
               .ToList();
    }

    public async Task<List<Menu>> GetTreeByUserIdAsync(long userId)
    {
        return TreeConverter.ConvertToTree((await _repository.GetAllByUserIdAsync(userId)).Where(x=>!(x.Meta.Type == MetaType.Api || x.Meta.Type == MetaType.Button)).ToList());
    }

    /// <summary>
    /// 树结构转换
    /// </summary>
    public static class TreeConverter
    {
        public static List<Menu> ConvertToTree(List<Menu> flatList)
        {
            var rootNodes = flatList.Where(node => !node.ParentId.HasValue).ToList();

            foreach (var rootNode in rootNodes)
            {
                PopulateChildren(rootNode, flatList);
            }

            return rootNodes;
        }

        private static void PopulateChildren(Menu parentNode, List<Menu> flatList)
        {
            var children = flatList.Where(node => node.ParentId == parentNode.Id).ToList();

            foreach (var childNode in children)
            {
                PopulateChildren(childNode, flatList);
            }
            if (children.Any())
            {
                if (parentNode.Children == null)
                    parentNode.Children = new List<Menu>();

                parentNode.Children.AddRange(children);
            }
        }
    }
}
