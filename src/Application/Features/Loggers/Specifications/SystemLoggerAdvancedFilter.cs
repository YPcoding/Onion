﻿namespace Application.Features.Loggers.Specifications;

/// <summary>
/// 高级查询
/// </summary>
public class SystemLoggerAdvancedFilter : PaginationFilter
{       
    /// <summary>
    /// 
    /// </summary>
    [Description("")]
    public string? Timestamp { get; set; }
       
    /// <summary>
    /// 
    /// </summary>
    [Description("")]
    public string? Level { get; set; }
       
    /// <summary>
    /// 
    /// </summary>
    [Description("")]
    public string? Template { get; set; }
       
    /// <summary>
    /// 
    /// </summary>
    [Description("")]
    public string? Message { get; set; }
       
    /// <summary>
    /// 
    /// </summary>
    [Description("")]
    public string? Exception { get; set; }
       
    /// <summary>
    /// 
    /// </summary>
    [Description("")]
    public string? Properties { get; set; }

    public DateTime? StartDateTime { get; set; }
    public DateTime? EndDateTime { get; set; }
}