using Domain.Entities.Job;
namespace Application.Features.ScheduledJobs.Specifications;

/// <summary>
/// 高级查询
/// </summary>
public class ScheduledJobAdvancedFilter : PaginationFilter
{       
    /// <summary>
    /// 任务名称
    /// </summary>
    [Description("任务名称")]
    public string? JobName { get; set; }
       
    /// <summary>
    /// 任务分组
    /// </summary>
    [Description("任务分组")]
    public string? JobGroup { get; set; }
       
    /// <summary>
    /// 触发器名称
    /// </summary>
    [Description("触发器名称")]
    public string? TriggerName { get; set; }
       
    /// <summary>
    /// 触发器分组
    /// </summary>
    [Description("触发器分组")]
    public string? TriggerGroup { get; set; }
       
    /// <summary>
    /// Cron表达式
    /// </summary>
    [Description("Cron表达式")]
    public string? CronExpression { get; set; }
       
    /// <summary>
    /// 相关的数据
    /// </summary>
    [Description("相关的数据")]
    public string? Data { get; set; }
       
    /// <summary>
    /// 最后执行消息
    /// </summary>
    [Description("最后执行消息")]
    public string? LastExecutionMessage { get; set; }
}