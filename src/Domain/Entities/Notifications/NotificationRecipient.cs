using System.ComponentModel;

namespace Domain.Entities.Notifications;

/// <summary>
/// 通知关联表
/// </summary>
public class NotificationRecipient : BaseEntity
{
    public int NotificationId { get; set; }
    public int RecipientId { get; set; }
    /// <summary>
    /// 通知状态（未读、已读、已删除）
    /// </summary>
    [Description("通知状态")]
    public NotificationStatus Status { get; set; }
    public Notification Notification { get; set; }
    public User Recipient { get; set; }
}
