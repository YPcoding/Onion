using System.ComponentModel;

namespace Domain.Entities.Notifications;

/// <summary>
/// 历史通知
/// </summary>
[Description("历史通知")]
public class HistoricalNotification : BaseEntity
{
    /// <summary>
    /// 通知标题
    /// </summary>
    [Description("通知标题")]
    public string Title { get; set; }

    /// <summary>
    /// 通知内容
    /// </summary>
    [Description("通知内容")]
    public string Content { get; set; }

    public int SenderId { get; set; }
    public User Sender { get; set; }

    /// <summary>
    /// 通知类型
    /// </summary>
    [Description("通知类型")]
    public NotificationType NotificationType { get; set; }

    /// <summary>
    /// 相关链接
    /// </summary>
    [Description("相关链接")]
    public string Link { get; set; }

    /// <summary>
    /// 通知状态
    /// </summary>
    [Description("通知状态")]
    public NotificationStatus Status { get; set; }
}




