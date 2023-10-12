using Quartz;
using System.ComponentModel;

namespace Domain.Enums;

/// <summary>
/// 定时任务状态
/// </summary>
[Description("定时任务状态")]
public enum JobStatus
{
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
    /// 任务已成功完成。
    /// </summary>
    [Description("完成")]
    Completed,

    /// <summary>
    /// 执行错误
    /// </summary>
    [Description("执行错误")]
    Error,

    /// <summary>
    /// 阻塞
    /// </summary>
    [Description("阻塞")]
    Blocked,

    /// <summary>
    /// 无此任务
    /// </summary>
    [Description("无此任务")]
    None,
}