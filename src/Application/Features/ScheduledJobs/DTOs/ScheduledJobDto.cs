using Domain.Entities.Job;
using Domain.Entities.Loggers;

namespace Application.Features.ScheduledJobs.DTOs;

[Map(typeof(ScheduledJob))]
public class ScheduledJobDto
{    

    /// <summary>
    /// 任务名称
    /// </summary>
    [Description("任务名称")]
    public string JobName { get; set; }    

    /// <summary>
    /// 任务分组
    /// </summary>
    [Description("任务分组")]
    public string JobGroup { get; set; }    

    /// <summary>
    /// 触发器名称
    /// </summary>
    [Description("触发器名称")]
    public string TriggerName { get; set; }    

    /// <summary>
    /// 触发器分组
    /// </summary>
    [Description("触发器分组")]
    public string TriggerGroup { get; set; }    

    /// <summary>
    /// Cron表达式
    /// </summary>
    [Description("Cron表达式")]
    public string CronExpression { get; set; }    

    /// <summary>
    /// 执行状态
    /// </summary>
    [Description("执行状态")]
    public JobStatus? Status { get; set; }    

    /// <summary>
    /// 相关的数据
    /// </summary>
    [Description("相关的数据")]
    public string Data { get; set; }    

    /// <summary>
    /// 最后执行时间
    /// </summary>
    [Description("最后执行时间")]
    public DateTimeOffset? LastExecutionTime { get; set; }    

    /// <summary>
    /// 下一次执行时间
    /// </summary>
    [Description("下一次执行时间")]
    public DateTimeOffset? NextExecutionTime { get; set; }    

    /// <summary>
    /// 最后执行状态
    /// </summary>
    [Description("最后执行状态")]
    public ExecutionStatus? LastExecutionStatus { get; set; }    

    /// <summary>
    /// 最后执行消息
    /// </summary>
    [Description("最后执行消息")]
    public string LastExecutionMessage { get; set; }     

    /// <summary>
    /// 唯一标识
    /// </summary>
    public long ScheduledJobId 
    {
        get 
        {
            return Id;
        }
    }    

    /// <summary>
    /// 唯一标识
    /// </summary>
    [Description("唯一标识")]
    public long Id { get; set; }    

    /// <summary>
    /// 乐观并发标记
    /// </summary>
    [Description("乐观并发标记")]
    public string ConcurrencyStamp { get; set; }
}

public class JobGroupDto 
{
    public string Label { get; set; }
    public string Value { get; set; }
    public string? ParameterJson { get; set; }
    public string? Description { get; set; }
}

[Map(typeof(Logger))]
public class ScheduledJobLogDto 
{
    public long Id { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public long? TimestampLong { get; set; }
    public string? Level { get; set; }
    public string? MessageTemplate { get; set; }
    public string? Message { get; set; }
    public string? Exception { get; set; }
    public string? Properties { get; set; }
}