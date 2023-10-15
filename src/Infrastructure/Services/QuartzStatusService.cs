using Quartz;
using Domain.Enums;
using System.Reflection;
using Common.Quartzs;
using Domain.Entities.Job;
using Microsoft.AspNetCore.SignalR;
using Application.Common;

namespace Infrastructure.Services;

public class QuartzStatusUpdaterAndStarterService : ITransientDependency
{
    private readonly ISchedulerFactory _scheduler;
    private readonly IServiceProvider _serviceProvider;
    private readonly IQuartzService _quartzService;
    private readonly IHubContext<SignalRHub> _hubContext;

    public QuartzStatusUpdaterAndStarterService(ISchedulerFactory scheduler, IServiceProvider serviceProvider, IQuartzService quartzService, IHubContext<SignalRHub> hubContext)
    {
        _scheduler = scheduler;
        _serviceProvider = serviceProvider;
        _quartzService = quartzService;
        _hubContext = hubContext;
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
                List<ScheduledJob> scheduledJobToUpdateStatus = new List<ScheduledJob>();
                foreach (var scheduledjob in scheduledjobs) 
                {
                    if (scheduledjob.Status == JobStatus.None ||
                        scheduledjob.Status == JobStatus.Paused ||
                        scheduledjob.Status == JobStatus.Completed)
                    {
                        continue;
                    }
                    var status = MapToCustomStatus(await scheduler.GetTriggerState(new TriggerKey(scheduledjob.TriggerName!, scheduledjob.TriggerGroup!)));
                    if (scheduledjob.Status != status) 
                    {
                        scheduledjob.Status = status;
                        _dbContext.ScheduledJobs.Attach(scheduledjob);
                        _dbContext.Entry(scheduledjob).Property("Status").IsModified = true;
                    }
                }
                await _hubContext.Clients.All.SendAsync("ReceiveUpdateTaskStatusMessage");
                _dbContext.ScheduledJobs.UpdateRange(scheduledJobToUpdateStatus);
                if ((await _dbContext.SaveChangesAsync()) > 0)
                {
                    await _hubContext.Clients.All.SendAsync("ReceiveUpdateTaskStatusMessage");
                }
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
                                .UsingJobData("parameter", scheduledjob.Data)
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

