using Application.Constants.Loggers;
using Quartz;

namespace Application.Common.Jobs;

public static class JobLogger
{
    public static void Log(ILogger logger, IJobExecutionContext context, Exception ex = null)
    {
        if (logger == null || context == null) return;

        if (ex == null)
        {
            logger.LogInformation(MessageTemplate.ScheduledJobLog,
                                context?.JobDetail.Key.ToString(),
                                context?.PreviousFireTimeUtc?.LocalDateTime,
                                context?.NextFireTimeUtc?.LocalDateTime,
                                "执行成功", ExecutionStatus.Success);
        }
        else
        {
            logger.LogInformation(MessageTemplate.ScheduledJobLog,
                                context?.JobDetail.Key.ToString(),
                                context?.PreviousFireTimeUtc?.LocalDateTime,
                                context?.NextFireTimeUtc?.LocalDateTime,
                                ex.Message.ToString(), ExecutionStatus.Failure);
        }
    }
}