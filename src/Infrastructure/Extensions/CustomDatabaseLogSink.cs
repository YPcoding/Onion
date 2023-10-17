using Serilog.Core;
using Serilog.Events;

namespace Infrastructure.Extensions;

public class CustomDatabaseLogSink : ILogEventSink
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ICurrentUserService  _userService;
    public CustomDatabaseLogSink(
        IServiceProvider serviceProvider,
        ICurrentUserService userService)
    {
        _serviceProvider = serviceProvider;
        _userService = userService;
    }

    public void Emit(LogEvent logEvent)
    {
        if (logEvent == null) { return; }
        if (logEvent.MessageTemplate.ToString() == (Application.Constants.Loggers.MessageTemplate.ActivityHistoryLog))
        {
            SaveLogger(logEvent);
        }
        if (logEvent.MessageTemplate.ToString() == (Application.Constants.Loggers.MessageTemplate.ScheduledJobLog))
        {
            SaveJobLogger(logEvent);
        }
    }

    public void SaveLogger(LogEvent logEvent) 
    {
        var properties = new Dictionary<string, object>();
        foreach (var property in logEvent.Properties)
        {
            var value = property.Value.ToString();
            if (value is not null && value.StartsWith(@"""{")) 
            {
                properties[property.Key] = value;
                continue;
            }
            if (value is not null && value.StartsWith(@""""))
            {
                value = value.Replace(@"""", "");
                properties[property.Key] = value;
                continue;
            }
            properties[property.Key] = value;
        }

        Domain.Entities.Loggers.Logger logger = new Domain.Entities.Loggers.Logger()
        {
            UserId = _userService.CurrentUserId == 0 ? null : _userService.CurrentUserId,
            Level = logEvent.Level.ToString(),
            Message = logEvent.RenderMessage(),
            MessageTemplate = logEvent.MessageTemplate.ToString(),
            Exception = logEvent.Exception?.ToString(),
            Timestamp = logEvent.Timestamp,
            TimestampLong = logEvent.Timestamp.ToUnixTimestampMilliseconds(),
            Properties = properties.ToJson(),
        };


        using var scope = _serviceProvider.CreateScope();
        var _context = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
        _context.Loggers.Add(logger);
        _context.SaveChanges();
    }

    public void SaveJobLogger(LogEvent logEvent)
    {
        SaveLogger(logEvent);
    }
}