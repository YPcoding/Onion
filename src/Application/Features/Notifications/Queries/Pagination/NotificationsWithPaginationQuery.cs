using Application.Common.Extensions;
using Domain.Entities.Notifications;
using Application.Features.Notifications.Caching;
using Application.Features.Notifications.DTOs;
using Application.Features.Notifications.Specifications;

namespace Application.Features.Notifications.Queries.Pagination;

/// <summary>
/// 通知分页查询
/// </summary>
[Description("分页查询通知数据")]
public class NotificationsWithPaginationQuery : NotificationAdvancedFilter, IRequest<Result<PaginatedData<NotificationDto>>>
{
    [JsonIgnore]
    public NotificationAdvancedPaginationSpec Specification => new NotificationAdvancedPaginationSpec(this);
}

/// <summary>
/// 处理程序
/// </summary>
public class NotificationsWithPaginationQueryHandler :
    IRequestHandler<NotificationsWithPaginationQuery, Result<PaginatedData<NotificationDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    public NotificationsWithPaginationQueryHandler(
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
    /// <returns>返回通知分页数据</returns>
    public async Task<Result<PaginatedData<NotificationDto>>> Handle(
        NotificationsWithPaginationQuery request,
        CancellationToken cancellationToken)
    {
        var notifications = await _context.Notifications
            .OrderBy($"{request.OrderBy} {request.SortDirection}")
            .ProjectToPaginatedDataAsync<Notification, NotificationDto>(
            request.Specification,
            request.PageNumber,
            request.PageSize,
            _mapper.ConfigurationProvider,
            cancellationToken);

        return await Result<PaginatedData<NotificationDto>>.SuccessAsync(notifications);
    }
}
