using Domain.Entities;

namespace Application.Features.Roles.DTOs;

[Map(typeof(Permission))]
public class RolePermissionDto
{
    /// <summary>
    /// 权限唯一标识
    /// </summary>
    public long Id { get; set; }
    /// <summary>
    /// 权限唯一标识
    /// </summary>
    public long PermissionId 
    { 
        get 
        {
            return Id; 
        }
    }
    /// <summary>
    /// 权限上级唯一标识
    /// </summary>
    public long? SuperiorId { get; set; }
    /// <summary>
    /// 权限上级唯一标识
    /// </summary>
    public long? ParentId 
    { 
        get 
        { 
            return SuperiorId; 
        } 
    }
    /// <summary>
    /// 名称
    /// </summary>
    public string Label { get; set; }
    /// <summary>
    /// 是否拥有
    /// </summary>
    public bool Has { get; set; }
}
