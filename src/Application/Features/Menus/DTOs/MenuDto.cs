using Application.Features.Menus.Commands.AddEdit;
using Domain.ValueObjects;

namespace Application.Features.Menus.DTOs;

[Map(typeof(Menu))]
public class MenuDto
{    

    /// <summary>
    /// 父级节点
    /// </summary>
    [Description("父级节点")]
    public long? ParentId { get; set; }

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

    /// <summary>
    /// 元信息
    /// </summary>
    [Description("元信息")]
    public Meta? Meta { get; set; }    


    /// <summary>
    /// 唯一标识
    /// </summary>
    public long MenuId 
    {
        get 
        {
            return Id;
        }
    }    

    /// <summary>
    /// 唯一标识
    /// </summary>
    [Description("唯一标识")]
    public long Id { get; set; }    

    /// <summary>
    /// 乐观并发标记
    /// </summary>
    [Description("乐观并发标记")]
    public string ConcurrencyStamp { get; set; }

    /// <summary>
    /// 视图
    /// </summary>
    [Description("视图")]
    public string? Component { get; set; }

    [Map(typeof(List<Menu>))]
    public List<MenuDto> Children { get; set; }
}