using Domain.Entities.Audit;
namespace Application.Features.AuditTrails.EventHandlers;

public class AuditTrailCreatedEventHandler : INotificationHandler<CreatedEvent<AuditTrail>>
{
    private readonly ILogger<AuditTrailCreatedEventHandler> _logger;

    public AuditTrailCreatedEventHandler(
        ILogger<AuditTrailCreatedEventHandler> logger
    )
    {
        _logger = logger;
    }

    public Task Handle(CreatedEvent<AuditTrail> notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
