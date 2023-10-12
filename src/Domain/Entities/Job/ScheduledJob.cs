using System.ComponentModel;

namespace Domain.Entities.Job
{
    /// <summary>
    /// 定时任务
    /// </summary>
    [Description("定时任务")]
    public class ScheduledJob : BaseAuditableSoftDeleteEntity, IAuditTrial
    {
        /// <summary>
        /// 任务名称。
        /// </summary>
        [Description("任务名称")]
        public virtual string? JobName { get; set; }

        /// <summary>
        /// 任务分组。
        /// </summary>
        [Description("任务分组")]
        public virtual string? JobGroup { get; set; }

        /// <summary>
        /// 触发器名称。
        /// </summary>
        [Description("触发器名称")]
        public virtual string? TriggerName { get; set; }

        /// <summary>
        /// 触发器分组。
        /// </summary>
        [Description("触发器分组")]
        public virtual string? TriggerGroup { get; set; }

        /// <summary>
        /// Cron表达式。
        /// </summary>
        [Description("Cron表达式")]
        public virtual string? CronExpression { get; set; }

        /// <summary>
        /// 执行状态。
        /// </summary>
        [Description("执行状态")]
        public virtual JobStatus? Status { get; set; }

        /// <summary>
        /// 相关的数据。
        /// </summary>
        [Description("相关的数据")]
        public virtual string? Data { get; set; }

        /// <summary>
        /// 最后执行时间。
        /// </summary>
        [Description("上次执行时间")]
        public virtual DateTimeOffset? LastExecutionTime { get; set; }

        /// <summary>
        /// 下一次执行时间。
        /// </summary>
        [Description("下一次执行时间")]
        public virtual DateTimeOffset? NextExecutionTime { get; set; }

        /// <summary>
        /// 最后执行状态。
        /// </summary>
        [Description("最后执行状态")]
        public virtual ExecutionStatus? LastExecutionStatus { get; set; }

        /// <summary>
        /// 最后执行消息。
        /// </summary>
        [Description("最后执行消息")]
        public virtual string? LastExecutionMessage { get; set; }
    }
}
