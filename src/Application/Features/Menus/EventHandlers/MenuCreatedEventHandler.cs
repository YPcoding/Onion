using Domain.Entities.Identity;
namespace Application.Features.Menus.EventHandlers;

public class MenuCreatedEventHandler : INotificationHandler<CreatedEvent<Menu>>
{
    private readonly ILogger<MenuCreatedEventHandler> _logger;

    public MenuCreatedEventHandler(
        ILogger<MenuCreatedEventHandler> logger
    )
    {
        _logger = logger;
    }

    public Task Handle(CreatedEvent<Menu> notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
