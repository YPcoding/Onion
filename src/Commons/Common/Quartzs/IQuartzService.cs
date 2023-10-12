using Common.Interfaces;
using Quartz;

namespace Common.Quartzs;

/// <summary>
/// 定义Quartz定时任务服务的接口
/// </summary>
public interface IQuartzService : ISingletonDependency
{
    /// <summary>
    /// 启动Quartz服务
    /// </summary>
    Task StartAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 停止Quartz服务
    /// </summary>
    Task StopAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 添加一个定时任务
    /// </summary>
    /// <param name="jobDetail">任务详情</param>
    /// <param name="trigger">触发器</param>
    Task AddJobAsync(IJobDetail jobDetail, ITrigger trigger, CancellationToken cancellationToken = default);

    /// <summary>
    /// 暂停指定任务
    /// </summary>
    /// <param name="jobKey">任务键</param>
    Task PauseJobAsync(JobKey jobKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// 恢复执行指定任务
    /// </summary>
    /// <param name="jobKey">任务键</param>
    Task ResumeJobAsync(JobKey jobKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除指定任务
    /// </summary>
    /// <param name="jobKey">任务键</param>
    Task DeleteJobAsync(JobKey jobKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查任务状态
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> CheckExistsAsync(JobKey jobKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取任务状态
    /// </summary>
    /// <param name="triggerKey"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<TriggerState> GetJobStateAsync(TriggerKey triggerKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// 停止某个触发器的执行
    /// </summary>
    /// <param name="triggerKey"></param>
    /// <returns></returns>
    Task UnscheduleJobAsync(TriggerKey triggerKey, CancellationToken cancellationToken = default);
}
