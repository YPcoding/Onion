using Quartz;

namespace Application.Common.Jobs;

[Description("定时任务示例")]
[DisallowConcurrentExecution]
public class ExampleJob : IJob, ITransientDependency,IDisposable
{
    // 这里可以定义需要清理的资源或状态
    private bool disposed = false;
    private string? managedResource = "定时任务示例";

    public ExampleJob()
    {
        // 构造函数逻辑（如果有的话）
    }

    private readonly IServiceProvider _serviceProvider;

    public ExampleJob(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// 执行逻辑
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public Task Execute(IJobExecutionContext context)
    {
        Console.WriteLine(context.ToReadableJson());
        return Task.CompletedTask;
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
                if (managedResource != null)
                {
                    managedResource = null; // 将托管资源设置为 null 或进行其他清理操作
                }
            }

            // 释放非托管资源
            disposed = true;
        }
    }
}
