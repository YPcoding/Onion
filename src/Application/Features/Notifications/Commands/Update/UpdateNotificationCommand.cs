using Application.Features.Notifications.Caching;
using Domain.Entities.Notifications;
using Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.Notifications.Commands.Update;


/// <summary>
/// 修改通知
/// </summary>
[Map(typeof(Notification))]
[Description("修改通知")]
public class UpdateNotificationCommand : IRequest<Result<long>>
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
        [Description("唯一标识")]
        public long NotificationId { get; set; }
        
        /// <summary>
        /// 乐观并发标记
        /// </summary>
        [Description("乐观并发标记")]
        public string ConcurrencyStamp { get; set; }
}

/// <summary>
/// 处理程序
/// </summary>
public class UpdateNotificationCommandHandler : IRequestHandler<UpdateNotificationCommand, Result<long>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UpdateNotificationCommandHandler(
        IApplicationDbContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// 业务逻辑
    /// </summary>
    /// <param name="request">请求参数</param>
    /// <param name="cancellationToken">取消标记</param>
    /// <returns>返回处理结果</returns>
    public async Task<Result<long>> Handle(UpdateNotificationCommand request, CancellationToken cancellationToken)
    {
        var notification = await _context.Notifications
           .SingleOrDefaultAsync(x => x.Id == request.NotificationId, cancellationToken)
           ?? throw new NotFoundException($"数据【{request.NotificationId}】未找到");

        notification = _mapper.Map(request, notification);
        //notification.AddDomainEvent(new UpdatedEvent<Notification>(notification));
        _context.Notifications.Update(notification);
        var isSuccess = await _context.SaveChangesAsync(cancellationToken) > 0;
        return await Result<long>.SuccessOrFailureAsync(notification.Id, isSuccess, new string[] { "操作失败" });
    }
}
