using System.ComponentModel;

namespace Domain.Entities.Notifications;

/// <summary>
/// 通知
/// </summary>
[Description("通知")]
public class Notification : BaseAuditableSoftDeleteEntity, IAuditTrial
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

    /// <summary>
    /// 发送者
    /// </summary>
    [Description("发送者")]
    public long? SenderId { get; set; }
    public User? Sender { get; set; }

    /// <summary>
    /// 通知类型
    /// </summary>
    [Description("通知类型")]
    public NotificationType NotificationType { get; set; }

    /// <summary>
    /// 相关链接
    /// </summary>
    [Description("相关链接")]
    public string? Link { get; set; }

    // 导航属性，表示通知与接收者的多对多关系
    public ICollection<NotificationRecipient> Recipients { get; set; } = new List<NotificationRecipient>();
}
