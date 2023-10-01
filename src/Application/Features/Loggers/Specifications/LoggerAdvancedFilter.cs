namespace Application.Features.Loggers.Specifications;

/// <summary>
/// 高级查询
/// </summary>
public class LoggerAdvancedFilter : PaginationFilter
{              
    /// <summary>
    /// 消息等级
    /// </summary>
    [Description("消息等级")]
    public string? Level { get; set; }
       
    /// <summary>
    /// 用户名
    /// </summary>
    [Description("用户名")]
    public string? UserName { get; set; }
       
    /// <summary>
    /// 客户端IP
    /// </summary>
    [Description("客户端IP")]
    public string? ClientIP { get; set; }
       
    /// <summary>
    /// IP
    /// </summary>
    [Description("IP")]
    public string? ClientAgent { get; set; }
}