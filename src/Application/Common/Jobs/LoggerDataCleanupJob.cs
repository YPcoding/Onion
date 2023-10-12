using Quartz;

namespace Application.Common.Jobs;

[Description("清理过期日志数据")]
[DisallowConcurrentExecution]
public class LoggerDataCleanupJob : IJob, ITransientDependency, IDisposable
{
    private bool disposed = false;
    public LoggerDataCleanupJob()
    {
        // 构造函数逻辑（如果有的话）
    }

    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ExampleJob> _logger;

    public LoggerDataCleanupJob(
        IServiceProvider serviceProvider, 
        ILogger<ExampleJob> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
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
            using (var scope = _serviceProvider.CreateScope())
            {
                var _dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
                var logsToDelete = await _dbContext.Loggers.ToListAsync();

                if (logsToDelete.Any())
                {
                    _dbContext.Loggers.RemoveRange(logsToDelete);
                    await _dbContext.SaveChangesAsync();
                }
            }
            JobLogger.Log(_logger, context);
        }
        catch (Exception ex)
        {
            JobLogger.Log(_logger, context, ex);
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