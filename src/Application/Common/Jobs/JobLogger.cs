using Application.Constants.Loggers;
using Quartz;

namespace Application.Common.Jobs;

/// <summary>
/// 定时任务日志记录
/// </summary>
public static class JobLogger
{
    /// <summary>
    /// 定时任务日志记录
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="context"></param>
    /// <param name="serviceProvider"></param>
    /// <param name="ex"></param>
    /// <param name="message"></param>
    public static async Task Log(ILogger logger, IJobExecutionContext context, IServiceProvider? serviceProvider = null, Exception? ex = null, string? message = "执行成功")
    {
        if (logger == null || context == null) return;

        var executionStatus = ExecutionStatus.Success;

        if (ex == null)
        {
            logger.LogInformation(MessageTemplate.ScheduledJobLog,
                                context?.JobDetail.Key.ToString(),
                                context?.PreviousFireTimeUtc.Value.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"),
                                context?.NextFireTimeUtc.Value.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"),
                                message, executionStatus);
        }
        else
        {
            message = ex.Message.ToString();
            executionStatus = ExecutionStatus.Failure;
            logger.LogError(MessageTemplate.ScheduledJobLog,
                                context?.JobDetail.Key.ToString(),
                                context?.PreviousFireTimeUtc.Value.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"),
                                context?.NextFireTimeUtc.Value.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"),
                                message, executionStatus);
        }
        if (serviceProvider != null)
        {
            using var scope = serviceProvider.CreateScope();
            var _dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
            var job = await _dbContext.ScheduledJobs.FirstOrDefaultAsync(x => (x.JobGroup + "." + x.JobName) == context!.JobDetail.Key.ToString());
            if (job == null) return;

            job.LastExecutionTime = context.PreviousFireTimeUtc.Value.ToLocalTime();
            job.NextExecutionTime = context.NextFireTimeUtc.Value.ToLocalTime();
            job.LastExecutionMessage = message;
            job.LastExecutionStatus = executionStatus;
            _dbContext.ScheduledJobs.Attach(job);
            _dbContext.Entry(job).Property("LastExecutionTime").IsModified = true;
            _dbContext.Entry(job).Property("NextExecutionTime").IsModified = true;
            _dbContext.Entry(job).Property("LastExecutionMessage").IsModified = true;
            _dbContext.Entry(job).Property("LastExecutionStatus").IsModified = true;
            await _dbContext.SaveChangesAsync();
        }
    }
}