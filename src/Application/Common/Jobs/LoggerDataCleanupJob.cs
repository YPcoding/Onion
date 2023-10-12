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

    public LoggerDataCleanupJob(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// 执行逻辑
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task Execute(IJobExecutionContext context)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            //var _dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
            //var logsToDelete = await _dbContext.Loggers.ToListAsync();

            //if (logsToDelete.Any())
            //{
            //    _dbContext.Loggers.RemoveRange(logsToDelete);
            //    await _dbContext.SaveChangesAsync();
            //}
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