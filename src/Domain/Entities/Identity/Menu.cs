using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Identity;

/// <summary>
/// 菜单管理
/// </summary>
[Description("菜单管理")]
public class Menu : BaseAuditableSoftDeleteEntity, IAuditTrial
{
    public Menu()
    {
        RoleMenus = new HashSet<RoleMenu>();
    }

    /// <summary>
    /// 父级节点
    /// </summary>
    [Description("父级节点")]
    public virtual long? ParentId { get; set; }
    [ForeignKey("ParentId")]
    public virtual Menu? Parent { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    [Description("名称")]
    public virtual string? Name { get; set; }

    /// <summary>
    /// 路径
    /// </summary>
    [Description("路径")]
    public virtual string? Path { get; set; }

    /// <summary>
    /// 重定向
    /// </summary>
    [Description("重定向")]
    public virtual string? Redirect { get; set; }

    /// <summary>
    /// 菜单高亮
    /// </summary>
    [Description("菜单高亮")]
    public virtual string? Active { get; set; }

    /// <summary>
    /// 视图
    /// </summary>
    [Description("视图")]
    public virtual string? Component { get; set; }

    /// <summary>
    /// 标识
    /// </summary>
    [Description("标识")]
    public virtual string? Code { get; set; }

    /// <summary>
    /// 接口地址
    /// </summary>
    [Description("接口地址")]
    public virtual string? Url { get; set; }

    /// <summary>
    /// 接口请求方式
    /// </summary>
    [Description("接口请求方式")]
    public virtual string? HttpMethods { get; set; }

    /// <summary>
    /// 元信息
    /// </summary>
    [Description("元信息")]
    public virtual Meta? Meta { get; set; }

    public ICollection<RoleMenu> RoleMenus { get; set; }

    [NotMapped]
    public List<Menu> Children { get; set; } = new List<Menu>();
}
