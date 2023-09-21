using Domain.Entities;

namespace Application.Features.Roles.DTOs;

/// <summary>
/// 角色信息
/// </summary>
[Map(typeof(Role))]
public class RoleDto
{
    /// <summary>
    /// 唯一标识
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 角色唯一标识
    /// </summary>
    public long RoleId
    { 
        get 
        { 
            return Id; 
        } 
    }

    /// <summary>
    /// 角色名称
    /// </summary>
    public string RoleName { get; set; }

    /// <summary>
    /// 角色描述
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime? Created { get; set; }

    /// <summary>
    /// 并发标记
    /// </summary>
    public string? ConcurrencyStamp { get; set; }
}
