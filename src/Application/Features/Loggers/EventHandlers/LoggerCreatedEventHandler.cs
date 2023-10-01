using Domain.Entities.Logger;
namespace Application.Features.Loggers.EventHandlers;

public class LoggerCreatedEventHandler : INotificationHandler<CreatedEvent<Logger>>
{
    private readonly ILogger<LoggerCreatedEventHandler> _logger;

    public LoggerCreatedEventHandler(
        ILogger<LoggerCreatedEventHandler> logger
    )
    {
        _logger = logger;
    }

    public Task Handle(CreatedEvent<Logger> notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
