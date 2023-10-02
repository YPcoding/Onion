using Domain.Entities.Notifications;
namespace Application.Features.Notifications.Specifications;

/// <summary>
/// 高级查询
/// </summary>
public class NotificationAdvancedFilter : PaginationFilter
{       
    /// <summary>
    /// 通知标题
    /// </summary>
    [Description("通知标题")]
    public string? Title { get; set; }
       
    /// <summary>
    /// 通知内容
    /// </summary>
    [Description("通知内容")]
    public string? Content { get; set; }
       
    /// <summary>
    /// 相关链接
    /// </summary>
    [Description("相关链接")]
    public string? Link { get; set; }
}