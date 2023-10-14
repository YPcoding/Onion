namespace Application.Features.Loggers.Specifications;

/// <summary>
/// 高级查询
/// </summary>
public class SystemLoggerAdvancedFilter : PaginationFilter
{              
    /// <summary>
    /// 日志等级
    /// </summary>
    [Description("")]
    public string? Level { get; set; }

    public DateTime? StartDateTime { get; set; }
    public DateTime? EndDateTime { get; set; }
}