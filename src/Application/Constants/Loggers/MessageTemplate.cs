namespace Application.Constants.Loggers;

public static class MessageTemplate
{
    public const string ActivityHistoryLog = "{ID},{LoggerName},{UserAgent},{ResponseData},{RequestParams},{RequestPath},{RequestName},{RequestMethod},{UserName},{ClientIP},{ResponseStatusCode},{Message},{LoggerTime},{ElapsedMilliseconds}";
    public const string ScheduledJobLog = "{JobKey},{LastExecutionTime},{NextExecutionTime},{LastExecutionMessage},{LastExecutionStatus}";
}