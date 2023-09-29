using Domain.Entities.Audit;
namespace Application.Features.AuditTrails.Specifications;

/// <summary>
/// 高级查询
/// </summary>
public class AuditTrailAdvancedFilter : PaginationFilter
{       
    /// <summary>
    /// 表名
    /// </summary>
    [Description("表名")]
    public string? TableName { get; set; }
       
    /// <summary>
    /// 具有临时属性
    /// </summary>
    [Description("具有临时属性")]
    public bool? HasTemporaryProperties { get; set; }
}