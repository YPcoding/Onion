using Domain.Enums;

namespace Application.Features.AuditTrails.Specifications;

/// <summary>
/// 审计日志搜索字段
/// </summary>
public class AuditTrailAdvancedFilter : PaginationFilter
{
    /// <summary>
    /// 用户名
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// 审计类型
    /// </summary>
    public AuditType? AuditType { get; set; }

    /// <summary>
    /// 表名
    /// </summary>
    public string? TableName { get; set; }
}
