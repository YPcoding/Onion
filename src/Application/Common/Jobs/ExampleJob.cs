using Quartz;

namespace Application.Common.Jobs;

/// <summary>
/// 此为定时任务示例
/// </summary>
[Description("定时任务示例")]
[DisallowConcurrentExecution]
public class ExampleJob : JobParameterAbstractBase<ExampleJob.Parameter>, IJob, ITransientDependency, IDisposable
{
    /// <summary>
    /// 当需要有参构造函数时，需要加一个无参构造函数
    /// </summary>
    public ExampleJob()
    {
        // 构造函数逻辑（如果有的话）
    }

    // 这里可以定义需要清理的资源或状态
    private bool disposed = false;
    private string? managedResource = "定时任务示例";
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ExampleJob> _logger;

    /// <summary>
    /// 有参构造函数
    /// </summary>
    /// <param name="serviceProvider">获取依赖注入实例，（有些需要手动管理生命周期，如IApplicationDbContext）</param>
    /// <param name="logger">日志实例</param>
    public ExampleJob(
        IServiceProvider serviceProvider,
        ILogger<ExampleJob> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    /// <summary>
    /// 定时任务参数
    /// </summary>
    public class Parameter 
    {
        [Description("名称")]//此描述会在前端显示
        public string Name { get; set; } = "名称";//设置默认参数
        [Description("计数")]//此描述会在前端显示
        public int Conut { get; set; } = 200;
        [Description("时间")]//此描述会在前端显示
        public DateTime Time { get; set; } = DateTime.Now;
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
            // 获取定时任务参数方式，如果继承了JobParameterAbstractBase，可以通过这样的方式获取参数
            var parameters = GetParameters<Parameter>(context);
            Console.WriteLine($"参数：{parameters?.ToJson()}");
            //使用数据库方式
            using (var scope = _serviceProvider.CreateScope())
            {
                var _dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
            }
            Console.WriteLine("我是定时任务示例，业务逻辑写在这里");

           await JobLogger.Log(_logger, context, _serviceProvider,null,"执行成功");//日志记录器
        }
        catch (Exception ex)
        {
            await JobLogger.Log(_logger, context, _serviceProvider, ex);//日志记录器
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
