using Domain.Entities;
namespace Application.Features.TestTables.EventHandlers;

public class TestTableCreatedEventHandler : INotificationHandler<CreatedEvent<TestTable>>
{
    private readonly ILogger<TestTableCreatedEventHandler> _logger;

    public TestTableCreatedEventHandler(
        ILogger<TestTableCreatedEventHandler> logger
    )
    {
        _logger = logger;
    }

    public Task Handle(CreatedEvent<TestTable> notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
