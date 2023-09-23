namespace Application.Features.Users.Specifications;

/// <summary>
/// 高级查询
/// </summary>
public class RoleAdvancedFilter : PaginationFilter
{
    /// <summary>
    /// 角色名称
    /// </summary>
    public string? RoleName { get; set; }

    /// <summary>
    /// 角色标识
    /// </summary>
    public string? RoleCode { get; set; }

    /// <summary>
    /// 角色描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 是否激活
    /// </summary>
    public bool? IsActive { get; set; }
}