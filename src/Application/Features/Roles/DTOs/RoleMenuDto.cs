using Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.Roles.DTOs;

[Map(typeof(Menu))]
public class RoleMenuDto
{
    /// <summary>
    /// 唯一标识
    /// </summary>
    public long Id { get; set; }
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
    /// 上级唯一标识
    /// </summary>
    public long? ParentId { get; set; }

    /// <summary>
    /// 元信息
    /// </summary>
    public Meta Meta { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    public string? Title { get { return Meta.Title; } }

    /// <summary>
    /// 是否拥有
    /// </summary>
    public bool Has { get; set; }

    /// <summary>
    /// 权限类型
    /// </summary>
    public MetaType? Type { get { return Meta.Type; } }

    /// <summary>
    /// 并发标记
    /// </summary>
    [Required(ErrorMessage = "并发标记必填的")]
    public string ConcurrencyStamp { get; set; }
}
