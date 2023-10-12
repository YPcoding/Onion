using Domain.Enums;
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

    public void SaveJobLogger(LogEvent logEvent)
    {
        var jobDetail = "";
        DateTimeOffset? lastExecutionTime = null;
        DateTimeOffset? nextExecutionTime = null;
        var lastExecutionMessage = "";
        ExecutionStatus lastExecutionStatus = ExecutionStatus.Success;
        var jobGourp = "";
        var jobName = "";

        var propertiesDictionary = new Dictionary<string, string>();
        foreach (var property in logEvent.Properties)
        {
            if (property.Value == null) continue;
            if (property.Value.ToString() == "null") continue;

            if (property.Key == "JobDetail")
            {
                jobDetail = property.Value.ToString();
            }
            if (property.Key == "LastExecutionTime")
            {
                lastExecutionTime = DateTimeOffset.Parse(property.Value.ToString());
            }
            if (property.Key == "NextExecutionTime")
            {
                nextExecutionTime = DateTimeOffset.Parse(property.Value.ToString());
            }
            if (property.Key == "LastExecutionMessage")
            {
                lastExecutionMessage = property.Value.ToString();
            }
            if (property.Key == "LastExecutionStatus")
            {
                if (property.Value.ToString() == "Success")
                {
                    lastExecutionStatus = ExecutionStatus.Success;
                }
                else
                {
                    lastExecutionStatus = ExecutionStatus.Failure;
                }
            }
        }

        string? fullString = jobDetail.Replace("\"", "");
        int index = fullString?.LastIndexOf('.') ?? -1;
        if (index >= 0)
        {
            jobGourp = fullString!.Substring(0, index);
            jobName = fullString!.Substring(index + 1);
            var jop = _dbContext.ScheduledJobs.FirstOrDefault(x => x.JobName == jobName && x.JobGroup == jobGourp);
            if (jop != null)
            {
                jop.LastExecutionTime = lastExecutionTime;
                jop.NextExecutionTime = nextExecutionTime;
                jop.LastExecutionMessage = lastExecutionMessage;
                jop.LastExecutionStatus = lastExecutionStatus;
                _dbContext.ScheduledJobs.Update(jop);
                _dbContext.SaveChanges();
            }
        }
        SaveLogger(logEvent);
    }
}