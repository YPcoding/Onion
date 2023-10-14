using Quartz;

namespace Application.Common.Jobs;

[Description("清理过期日志数据")]
[DisallowConcurrentExecution]
public class LoggerDataCleanupJob : JobParameterAbstractBase<LoggerDataCleanupJob.Parameter>, IJob, ITransientDependency, IDisposable
{
    private bool disposed = false;
    public LoggerDataCleanupJob()
    {
        // 构造函数逻辑（如果有的话）
    }

    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<LoggerDataCleanupJob> _logger;

    public LoggerDataCleanupJob(
        IServiceProvider serviceProvider,
        ILogger<LoggerDataCleanupJob> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    /// <summary>
    /// 定时任务参数
    /// </summary>
    public class Parameter
    {
        [Description("保留几天")]
        public int RetentionDays { get; set; } = 7;
    }

    /// <summary>
    /// 执行逻辑
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            int deleteCount = 0;
            using (var scope = _serviceProvider.CreateScope())
            {
                var parameters = GetParameters<Parameter>(context);
                if (parameters != null)
                {
                    var retentionDays = parameters.RetentionDays;
                    var timestamp = DateTimeOffset.Now.AddDays(-retentionDays).ToUnixTimestampMilliseconds();

                    var _dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
                    var logsToDelete = await _dbContext.Loggers
                        .Where(x => x.TimestampLong < timestamp)
                        .ToListAsync();

                    deleteCount = logsToDelete.Count;
                    if (deleteCount > 0)
                    {
                        _dbContext.Loggers.RemoveRange(logsToDelete);
                        await _dbContext.SaveChangesAsync();
                    }
                }
            }
            await JobLogger.Log(_logger, context, _serviceProvider, null, $"删除了{deleteCount}条日志");
        }
        catch (Exception ex)
        {
            await JobLogger.Log(_logger, context, _serviceProvider, ex);
        }
    }

    public void Dispose()
    {
        // 在 Dispose 方法中释放资源
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// 释放托管资源 示例
    /// </summary>
    /// <param name="disposing"></param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                // 释放托管资源
            }

            // 释放非托管资源
            disposed = true;
        }
    }
}