using Application.Features.Notifications.Caching;
using Domain.Entities.Notifications;
using Domain.Entities;

namespace Application.Features.Notifications.Commands.Delete;

/// <summary>
/// 删除通知
/// </summary>
[Description("删除通知")]
public class DeleteNotificationCommand : IRequest<Result<bool>>
{
  
        /// <summary>
        /// 唯一标识
        /// </summary>
        [Description("唯一标识")]
        public List<long> NotificationIds { get; set; }
}

/// <summary>
/// 处理程序
/// </summary>
public class DeleteNotificationCommandHandler : IRequestHandler<DeleteNotificationCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public DeleteNotificationCommandHandler(
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
    public async Task<Result<bool>> Handle(DeleteNotificationCommand request, CancellationToken cancellationToken)
    {
        var notificationsToDelete = await _context.Notifications
            .Where(x => request.NotificationIds.Contains(x.Id))
            .ToListAsync();

        if (notificationsToDelete.Any())
        {
            _context.Notifications.RemoveRange(notificationsToDelete);
            var isSuccess = await _context.SaveChangesAsync(cancellationToken) > 0;
            return await Result<bool>.SuccessOrFailureAsync(isSuccess, isSuccess,new string[] {"操作失败"});
        }

        return await Result<bool>.FailureAsync(new string[] { "没有找到需要删除的数据" });
    }
}