using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Domain.Enums;

/// <summary>
/// 通知类型
/// </summary>
[Description("通知类型")]
public enum NotificationType
{
    /// <summary>
    /// 系统通知
    /// </summary>
    [Description("系统通知")]
    System,
    /// <summary>
    /// 个人通知
    /// </summary>
    [Description("个人通知")]
    Personal 
}

/// <summary>
/// 通知状态（未读、已读、已删除）
/// </summary>
[Description("通知状态")]
public enum NotificationStatus
{
    /// <summary>
    /// 未读
    /// </summary>
    [Description("未读")]
    Unread,
    /// <summary>
    /// 已读
    /// </summary>
    [Description("已读")]
    Read,
    /// <summary>
    /// 已删除
    /// </summary>
    [Description("已删除")]
    Deleted
}
