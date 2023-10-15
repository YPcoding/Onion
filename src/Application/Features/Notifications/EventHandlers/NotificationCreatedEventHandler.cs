using Application.Common;
using Domain.Entities.Notifications;
using Microsoft.AspNetCore.SignalR;

namespace Application.Features.Notifications.EventHandlers;

public class NotificationCreatedEventHandler : INotificationHandler<CreatedEvent<Notification>>
{
    private readonly ILogger<NotificationCreatedEventHandler> _logger;
    private readonly IHubContext<SignalRHub> _hubContext;
    private readonly IServiceProvider _serviceProvider; // 添加此字段


    public NotificationCreatedEventHandler(
        ILogger<NotificationCreatedEventHandler> logger,
        IHubContext<SignalRHub> hubContext,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _hubContext = hubContext;
        _serviceProvider = serviceProvider;
    }

    public async Task Handle(CreatedEvent<Notification> notification, CancellationToken cancellationToken)
    {
        var sender = new Notification();
        using (var scope = _serviceProvider.CreateScope())
        {
            var _context = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
            sender = await _context.Notifications.Include(e=>e.Sender).SingleOrDefaultAsync(n => n.Id == notification.Entity.Id);
        }
        await _hubContext.Clients.All.SendAsync("ReceiveNotification", sender?.ToJsonWithSensitiveFilter());
        _logger.LogInformation($"ReceiveNotification:{sender?.ToJsonWithSensitiveFilter()}发送成功");
    }
}
