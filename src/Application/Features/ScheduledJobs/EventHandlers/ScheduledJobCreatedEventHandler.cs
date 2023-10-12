using Domain.Entities.Job;
namespace Application.Features.ScheduledJobs.EventHandlers;

public class ScheduledJobCreatedEventHandler : INotificationHandler<CreatedEvent<ScheduledJob>>
{
    private readonly ILogger<ScheduledJobCreatedEventHandler> _logger;

    public ScheduledJobCreatedEventHandler(
        ILogger<ScheduledJobCreatedEventHandler> logger
    )
    {
        _logger = logger;
    }

    public Task Handle(CreatedEvent<ScheduledJob> notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
