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
    /// 角色描述
    /// </summary>
    public string? Description { get; set; }
}