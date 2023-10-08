using Domain.Entities.Identity;
namespace Application.Features.Menus.Specifications;

/// <summary>
/// 高级查询
/// </summary>
public class MenuAdvancedFilter : PaginationFilter
{       
    /// <summary>
    /// 名称
    /// </summary>
    [Description("名称")]
    public string? Name { get; set; }
       
    /// <summary>
    /// 路径
    /// </summary>
    [Description("路径")]
    public string? Path { get; set; }
       
    /// <summary>
    /// 重定向
    /// </summary>
    [Description("重定向")]
    public string? Redirect { get; set; }
       
    /// <summary>
    /// 菜单高亮
    /// </summary>
    [Description("菜单高亮")]
    public string? Active { get; set; }
}