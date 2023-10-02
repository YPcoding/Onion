using Domain.Entities;
using Domain.Enums;
using Domain.Entities.Notifications;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.Notifications.DTOs
{
    [Map(typeof(Notification))]
    public class NotificationDto
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

        /// <summary>
        /// 
        /// </summary>
        [Description("")]
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
        /// 
        /// </summary>
        [Description("")]
        public ICollection<NotificationRecipient> Recipients { get; set; }     

        /// <summary>
        /// 唯一标识
        /// </summary>
        public long NotificationId 
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
}