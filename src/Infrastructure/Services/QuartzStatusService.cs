using Quartz;
using Domain.Enums;
using System.Reflection;
using Common.Quartzs;

namespace Infrastructure.Services;

public class QuartzStatusUpdaterAndStarterService : ITransientDependency
{
    private readonly ISchedulerFactory _scheduler;
    private readonly IServiceProvider _serviceProvider;
    private readonly IQuartzService _quartzService;

    public QuartzStatusUpdaterAndStarterService(ISchedulerFactory scheduler, IServiceProvider serviceProvider, IQuartzService quartzService)
    {
        _scheduler = scheduler;
        _serviceProvider = serviceProvider;
        _quartzService = quartzService;
    }

    public async Task UpdateTaskStatusInDatabase()
    {
        await StartSchedulerIfNotRunning();

        var scheduler = await _scheduler.GetScheduler();

        using (var scope = _serviceProvider.CreateScope())
        {
            var _dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
            var scheduledjobs = await _dbContext.ScheduledJobs.ToListAsync();
            if (scheduledjobs.Any())
            {
                foreach (var scheduledjob in scheduledjobs) 
                {
                    if (scheduledjob.Status == JobStatus.None ||
                        scheduledjob.Status == JobStatus.Paused ||
                        scheduledjob.Status == JobStatus.Completed)
                    {
                        continue;
                    }
                    scheduledjob.Status = MapToCustomStatus(await scheduler.GetTriggerState(new TriggerKey(scheduledjob.TriggerName!, scheduledjob.TriggerGroup!)));
                }
                _dbContext.ScheduledJobs.UpdateRange(scheduledjobs);
                await _dbContext.SaveChangesAsync();
            }
        }
    }

    public async Task StartSchedulerIfNotRunning()
    {
        var scheduler = await _scheduler.GetScheduler();
        if (!scheduler.IsStarted)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var _dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
                var scheduledjobs = await _dbContext.ScheduledJobs
                    .Where(x => !(
                    x.Status == JobStatus.None ||
                    x.Status == JobStatus.Paused ||
                    x.Status == JobStatus.Completed))
                    .ToListAsync();
                if (scheduledjobs.Any())
                {
                    foreach (var scheduledjob in scheduledjobs) 
                    {
                        var jobKey = new JobKey(scheduledjob.JobName!, scheduledjob.JobGroup!);
                        if (!(await _quartzService.CheckExistsAsync(jobKey)))
                        {
                            var trigger = TriggerBuilder.Create()
                                .WithIdentity(scheduledjob.TriggerName!, scheduledjob.TriggerGroup!)
                                .WithCronSchedule(scheduledjob.CronExpression!)
                                .Build();

                            Assembly assembly = Assembly.Load(scheduledjob.JobGroup!.Split('.')[0].ToString());
                            var type = assembly.GetType(scheduledjob.JobGroup)!;
                            var job = JobBuilder.Create(type)
                                .WithIdentity(scheduledjob.JobName!, scheduledjob.JobGroup)
                                .Build();

                            await _quartzService.AddJobAsync(job, trigger);
                        }
                    }

                    // 如果调度器没有启动，就启动它
                    await scheduler.Start();
                }
            }
        }
    }

    private JobStatus MapToCustomStatus(TriggerState quartzTriggerState)
    {
        switch (quartzTriggerState)
        {
            case TriggerState.Normal:
                return JobStatus.Normal;
            case TriggerState.Paused:
                return JobStatus.Paused;
            case TriggerState.Complete:
                return JobStatus.Completed;
            case TriggerState.Error:
                return JobStatus.Error;
            case TriggerState.Blocked:
                return JobStatus.Blocked;
            case TriggerState.None:
                return JobStatus.None;
            default:
                return JobStatus.None;
        }
    }
}

