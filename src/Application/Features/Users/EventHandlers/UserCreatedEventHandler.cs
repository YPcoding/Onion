using Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Users.EventHandlers;

public class UserCreatedEventHandler : INotificationHandler<CreatedEvent<User>>
{
    private readonly ILogger<UserCreatedEventHandler> _logger;


    public UserCreatedEventHandler(
        ILogger<UserCreatedEventHandler> logger
    )
    {
        _logger = logger;
    }

    public Task Handle(CreatedEvent<User> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("begin recognition: {Id}", notification.Entity.Id);
        _logger.LogWarning("begin recognition: {Id}", notification.Entity.Id);
        return Task.CompletedTask;
    }
}
