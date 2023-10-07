namespace Application.Features.Users.Specifications;

/// <summary>
/// 高级查询
/// </summary>
public class UserAdvancedFilter : PaginationFilter
{
    /// <summary>
    /// 用户名
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 确认邮箱
    /// </summary>
    public bool? EmailConfirmed { get; set; }

    /// <summary>
    /// 手机号码
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// 已锁定
    /// </summary>
    public bool? LockoutEnabled { get; set; }

    /// <summary>
    /// 部门唯一标识
    /// </summary>
    public long? DepartmentId { get; set; }
}