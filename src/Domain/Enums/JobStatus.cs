using System.ComponentModel;

namespace Domain.Enums;

/// <summary>
/// 定时任务状态
/// </summary>
[Description("定时任务状态")]
public enum JobStatus
{
    /// <summary>
    /// 无此任务
    /// </summary>
    [Description("无此任务")]
    None,

    /// <summary>
    /// 正常
    /// </summary>
    [Description("正常")]
    Normal,

    /// <summary>
    /// 暂停
    /// </summary>
    [Description("暂停")]
    Paused,

    /// <summary>
    /// 阻塞
    /// </summary>
    [Description("阻塞")]
    Blocked,

    /// <summary>
    /// 任务等待执行。
    /// </summary>
    [Description("等待执行")]
    Pending,

    /// <summary>
    /// 任务正在执行。
    /// </summary>
    [Description("正在执行")]
    Active,

    /// <summary>
    /// 任务已停用。
    /// </summary>
    [Description("停用")]
    Inactive,

    /// <summary>
    /// 任务已成功完成。
    /// </summary>
    [Description("完成")]
    Completed,

    /// <summary>
    /// 任务执行失败。
    /// </summary>
    [Description("失败")]
    Failed
}
