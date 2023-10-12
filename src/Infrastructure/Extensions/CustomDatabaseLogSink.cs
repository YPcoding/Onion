using Serilog.Core;
using Serilog.Events;

namespace Infrastructure.Extensions;

public class CustomDatabaseLogSink : ILogEventSink
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService  _userService;
    public CustomDatabaseLogSink(
        IApplicationDbContext dbContext, 
        ICurrentUserService userService)
    {
        _dbContext = dbContext;
        _userService = userService;
    }

    public void Emit(LogEvent logEvent)
    {
        if (logEvent == null) { return; }
        if (logEvent.MessageTemplate.ToString()!= "{ID},{LoggerName},{UserAgent},{ResponseData},{RequestParams},{RequestPath},{RequestName},{RequestMethod},{UserName},{ClientIP},{ResponseStatusCode},{Message},{LoggerTime},{ElapsedMilliseconds}")
        {
            return;
        }

        var propertiesDictionary = new Dictionary<string, object>();
        foreach (var property in logEvent.Properties)
        {
            propertiesDictionary[property.Key] = property.Value;
        }

        var jsonProperties = propertiesDictionary.ToJson();
        jsonProperties = jsonProperties.Replace(@"{""Value"":", "");
        jsonProperties = jsonProperties.Replace("},", ",");
        jsonProperties = jsonProperties.Replace("}}", "}");

        Domain.Entities.Loggers.Logger logger = new Domain.Entities.Loggers.Logger()
        {
            UserId = _userService.CurrentUserId == 0 ? null : _userService.CurrentUserId,
            Level = logEvent.Level.ToString(),
            Message = logEvent.RenderMessage(),
            MessageTemplate = logEvent.MessageTemplate.ToString(),
            Exception = logEvent.Exception?.ToString(),
            Timestamp = logEvent.Timestamp,
            TimestampLong = logEvent.Timestamp.ToUnixTimestampMilliseconds(),
            Properties = jsonProperties,
        };

        _dbContext.Loggers.Add(logger);
        _dbContext.SaveChanges();
    }
}