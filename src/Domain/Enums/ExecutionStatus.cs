using System.ComponentModel;

namespace Domain.Enums;

/// <summary>
/// 表示任务的最后执行状态。
/// </summary>
[Description("执行状态")]
public enum ExecutionStatus
{
    /// <summary>
    /// 任务执行成功。
    /// </summary>
    [Description("成功")]
    Success,

    /// <summary>
    /// 任务执行失败。
    /// </summary>
    [Description("失败")]
    Failure
}
