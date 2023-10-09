using Application.Features.Notifications.Caching;
using Application.Features.Notifications.DTOs;
using Domain.Entities.Notifications;
using AutoMapper.QueryableExtensions;

namespace Application.Features.Notifications.Queries.GetAll;


[Description("查询所有通知")]
public class GetAllNotificationsQuery : IRequest<Result<IEnumerable<NotificationDto>>>
{
}

public class GetAllNotificationsQueryHandler :
    IRequestHandler<GetAllNotificationsQuery, Result<IEnumerable<NotificationDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAllNotificationsQueryHandler(
        IApplicationDbContext context,
        IMapper mapper
    )
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<NotificationDto>>> Handle(GetAllNotificationsQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.Notifications
            .ProjectTo<NotificationDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
        return await Result<IEnumerable<NotificationDto>>.SuccessAsync(data);
    }
}

