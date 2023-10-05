using Domain.Entities.Settings;
namespace Application.Features.UserProfileSettings.EventHandlers;

public class UserProfileSettingsCreatedEventHandler : INotificationHandler<CreatedEvent<UserProfileSetting>>
{
    private readonly ILogger<UserProfileSettingsCreatedEventHandler> _logger;

    public UserProfileSettingsCreatedEventHandler(
        ILogger<UserProfileSettingsCreatedEventHandler> logger
    )
    {
        _logger = logger;
    }

    public Task Handle(CreatedEvent<UserProfileSetting> notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
