using Quartz;
using System.Threading;

namespace Common.Quartzs;

/// <summary>
/// Quartz定时任务服务
/// </summary>
public class QuartzService : IQuartzService
{
    private readonly ISchedulerFactory _scheduler;

    /// <summary>
    /// 创建一个新的QuartzService实例
    /// </summary>
    /// <param name="scheduler">Quartz调度器</param>
    public QuartzService(ISchedulerFactory scheduler)
    {
        _scheduler = scheduler;
    }

    /// <summary>
    /// 启动Quartz服务
    /// </summary>
    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        var scheduler = await _scheduler.GetScheduler(cancellationToken);
        await scheduler.Start(cancellationToken);
    }

    /// <summary>
    /// 停止Quartz服务
    /// </summary>
    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        var scheduler = await _scheduler.GetScheduler(cancellationToken);
        await scheduler.Shutdown(waitForJobsToComplete: true, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// 添加一个定时任务
    /// </summary>
    /// <param name="jobDetail">任务详情</param>
    /// <param name="trigger">触发器</param>
    public async Task AddJobAsync(IJobDetail jobDetail, ITrigger trigger, CancellationToken cancellationToken = default)
    {
        var scheduler = await _scheduler.GetScheduler(cancellationToken);
        await scheduler.ScheduleJob(jobDetail, trigger);
    }

    /// <summary>
    /// 暂停指定任务
    /// </summary>
    /// <param name="jobKey">任务键</param>
    public async Task PauseJobAsync(JobKey jobKey, CancellationToken cancellationToken = default)
    {
        var scheduler = await _scheduler.GetScheduler(cancellationToken);
        await scheduler.PauseJob(jobKey, cancellationToken);
    }

    /// <summary>
    /// 恢复执行指定任务
    /// </summary>
    /// <param name="jobKey">任务键</param>
    public async Task ResumeJobAsync(JobKey jobKey, CancellationToken cancellationToken = default)
    {
        var scheduler = await _scheduler.GetScheduler(cancellationToken);
        await scheduler.ResumeJob(jobKey, cancellationToken);
    }

    /// <summary>
    /// 删除指定任务
    /// </summary>
    /// <param name="jobKey">任务键</param>
    public async Task DeleteJobAsync(JobKey jobKey, CancellationToken cancellationToken = default)
    {
        var scheduler = await _scheduler.GetScheduler(cancellationToken);
        await scheduler.DeleteJob(jobKey, cancellationToken);
    }

    /// <summary>
    /// 检查是否存在JobKey
    /// </summary>
    /// <param name="jobKey"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> CheckExistsAsync(JobKey jobKey, CancellationToken cancellationToken = default)
    {
        var scheduler = await _scheduler.GetScheduler(cancellationToken);
        return await scheduler.CheckExists(jobKey, cancellationToken);
    }

    /// <summary>
    /// 获取任务状态
    /// </summary>
    /// <param name="jobKey"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<TriggerState> GetJobStateAsync(TriggerKey triggerKey, CancellationToken cancellationToken = default)
    {
        var scheduler = await _scheduler.GetScheduler(cancellationToken);      
        return await scheduler.GetTriggerState(triggerKey);
    }

    /// <summary>
    /// 停止某个触发器的执行
    /// </summary>
    /// <param name="triggerKey"></param>
    /// <returns></returns>
    public async Task UnscheduleJobAsync(TriggerKey triggerKey, CancellationToken cancellationToken = default) 
    {
        var scheduler = await _scheduler.GetScheduler(cancellationToken);
        await scheduler.UnscheduleJob(triggerKey);
    }
}
