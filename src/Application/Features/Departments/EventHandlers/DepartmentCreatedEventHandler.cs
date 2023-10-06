using Domain.Entities.Departments;
namespace Application.Features.Departments.EventHandlers;

public class DepartmentCreatedEventHandler : INotificationHandler<CreatedEvent<Department>>
{
    private readonly ILogger<DepartmentCreatedEventHandler> _logger;

    public DepartmentCreatedEventHandler(
        ILogger<DepartmentCreatedEventHandler> logger
    )
    {
        _logger = logger;
    }

    public Task Handle(CreatedEvent<Department> notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
