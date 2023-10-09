using Application.Common.Extensions;
using Domain.Entities.Notifications;
using Application.Features.Notifications.Caching;
using Application.Features.Notifications.DTOs;
using Application.Features.Notifications.Specifications;
using AutoMapper.QueryableExtensions;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.Notifications.Queries.GetById;

/// <summary>
/// 通过唯一标识获取一条数据
/// </summary>
[Description("查询单条通知数据")]
public class GetNotificationQueryById : IRequest<Result<NotificationDto>>
{
    /// <summary>
    /// 唯一标识
    /// </summary>
    [Required(ErrorMessage = "唯一标识必填的")]
    public long NotificationId { get; set; }
}

/// <summary>
/// 处理程序
/// </summary>
public class GetNotificationByIdQueryHandler :IRequestHandler<GetNotificationQueryById, Result<NotificationDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetNotificationByIdQueryHandler(
        IApplicationDbContext context,
        IMapper mapper
        )
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// 业务逻辑
    /// </summary>
    /// <param name="request">请求参数</param>
    /// <param name="cancellationToken">取消标记</param>
    /// <returns>返回查询的一条数据</returns>
    /// <exception cref="NotFoundException">未找到数据移除处理</exception>
    public async Task<Result<NotificationDto>> Handle(GetNotificationQueryById request, CancellationToken cancellationToken)
    {
        var notification = await _context.Notifications.ApplySpecification(new NotificationByIdSpec(request.NotificationId))
                     .ProjectTo<NotificationDto>(_mapper.ConfigurationProvider)
                     .SingleOrDefaultAsync(cancellationToken) ?? throw new NotFoundException($"唯一标识: [{request.NotificationId}] 未找到");
        return await Result<NotificationDto>.SuccessAsync(notification);
    }
}
