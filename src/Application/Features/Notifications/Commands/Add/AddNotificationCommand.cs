using System.ComponentModel.DataAnnotations;
using Domain.Entities.Notifications;

namespace Application.Features.Notifications.Commands.Add;

/// <summary>
/// 添加通知
/// </summary>
[Map(typeof(Notification))]
[Description("新增通知")]
public class AddNotificationCommand : IRequest<Result<long>>
{
    /// <summary>
    /// 通知标题
    /// </summary>
    [Description("通知标题")]
    [Required(ErrorMessage = "通知标题是必填的")]
    public string Title { get; set; }

    /// <summary>
    /// 通知内容
    /// </summary>
    [Description("通知内容")]
    [Required(ErrorMessage = "通知内容是必填的")]
    public string Content { get; set; }


    /// <summary>
    /// 通知类型
    /// </summary>
    [Description("通知类型")]
    [Required(ErrorMessage = "通知类型是必填的")]
    public NotificationType NotificationType { get; set; }

    /// <summary>
    /// 相关链接
    /// </summary>
    [Description("相关链接")]
    public string? Link { get; set; }

}
/// <summary>
/// 处理程序
/// </summary>
public class AddNotificationCommandHandler : IRequestHandler<AddNotificationCommand, Result<long>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public AddNotificationCommandHandler(
        IApplicationDbContext context,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// 业务逻辑
    /// </summary>
    /// <param name="request">请求参数</param>
    /// <param name="cancellationToken">取消标记</param>
    /// <returns>返回处理结果</returns>
    public async Task<Result<long>> Handle(AddNotificationCommand request, CancellationToken cancellationToken)
    {
        var notification = _mapper.Map<Notification>(request);
        notification.SenderId = _currentUserService.CurrentUserId;
        notification.AddDomainEvent(new CreatedEvent<Notification>(notification));
        await _context.Notifications.AddAsync(notification);
        var isSuccess = await _context.SaveChangesAsync(cancellationToken) > 0;
        return await Result<long>.SuccessOrFailureAsync(notification.Id, isSuccess, new string[] { "操作失败" });
    }
}