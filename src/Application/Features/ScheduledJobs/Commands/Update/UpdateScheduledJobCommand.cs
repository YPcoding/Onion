using Common.Quartzs;
using Domain.Entities.Job;
using Quartz;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Application.Features.ScheduledJobs.Commands.Update;


/// <summary>
/// 修改定时任务
/// </summary>
[Map(typeof(ScheduledJob))]
public class UpdateScheduledJobCommand : IRequest<Result<long>>
{
    /// <summary>
    /// 唯一标识
    /// </summary>
    [Description("唯一标识")]
    [Required(ErrorMessage = "唯一标识是必填的")]
    public long ScheduledJobId { get; set; }

    /// <summary>
    /// 乐观并发标记
    /// </summary>
    [Description("乐观并发标记")]
    [Required(ErrorMessage = "乐观并发标记是必填的")]
    public string ConcurrencyStamp { get; set; }

    /// <summary>
    /// 任务名称
    /// </summary>
    [Description("任务名称")]
    [Required(ErrorMessage = "任务名称是必填的")]
    public string JobName { get; set; }

    /// <summary>
    /// 任务分组。
    /// </summary>
    [Description("任务分组")]
    [Required(ErrorMessage = "任务类是必填的")]
    public virtual string JobGroup { get; set; }

    /// <summary>
    /// Cron表达式
    /// </summary>
    [Description("Cron表达式")]
    [Required(ErrorMessage = "Cron表达式是必填的")]
    public string CronExpression { get; set; }

    /// <summary>
    /// 相关的数据
    /// </summary>
    [Description("相关的数据")]
    public string? Data { get; set; }

    /// <summary>
    /// 执行状态
    /// </summary>
    [Description("执行状态")]
    public virtual JobStatus? Status { get; set; }
}

/// <summary>
/// 修改定时任务
/// </summary>
[Map(typeof(ScheduledJob))]
public class UpdateScheduledJobStatusCommand : IRequest<Result<long>>
{
    /// <summary>
    /// 唯一标识
    /// </summary>
    [Description("唯一标识")]
    [Required(ErrorMessage = "唯一标识是必填的")]
    public long ScheduledJobId { get; set; }

    /// <summary>
    /// 乐观并发标记
    /// </summary>
    [Description("乐观并发标记")]
    [Required(ErrorMessage = "乐观并发标记是必填的")]
    public string ConcurrencyStamp { get; set; }


    /// <summary>
    /// 执行状态
    /// </summary>
    [Description("执行状态")]
    public virtual JobStatus? Status { get; set; }
}


/// <summary>
/// 处理程序
/// </summary>
public class UpdateScheduledJobCommandHandler : 
    IRequestHandler<UpdateScheduledJobCommand, Result<long>>,
    IRequestHandler<UpdateScheduledJobStatusCommand, Result<long>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IQuartzService _quartzService;

    public UpdateScheduledJobCommandHandler(
        IApplicationDbContext context,
        IMapper mapper,
        IQuartzService quartzService)
    {
        _context = context;
        _mapper = mapper;
        _quartzService = quartzService;
    }

    /// <summary>
    /// 业务逻辑
    /// </summary>
    /// <param name="request">请求参数</param>
    /// <param name="cancellationToken">取消标记</param>
    /// <returns>返回处理结果</returns>
    public async Task<Result<long>> Handle(UpdateScheduledJobCommand request, CancellationToken cancellationToken)
    {
        var scheduledjob = await _context.ScheduledJobs
           .SingleOrDefaultAsync(x => x.Id == request.ScheduledJobId, cancellationToken)
           ?? throw new NotFoundException($"数据【{request.ScheduledJobId}】未找到");

        var oldJobKey = new JobKey(scheduledjob.JobName!, scheduledjob.JobGroup);
        var oldTriggerKey = new TriggerKey(scheduledjob.TriggerName!, scheduledjob.TriggerGroup!);

        scheduledjob = _mapper.Map(request, scheduledjob);
        scheduledjob.TriggerName = scheduledjob.JobName;
        scheduledjob.TriggerGroup = scheduledjob.JobGroup;

        Assembly assembly = Assembly.Load(request.JobGroup.Split('.')[0].ToString());

        if (assembly == null)
        {
            return await Result<long>.SuccessOrFailureAsync(scheduledjob.Id, false, new string[] { "操作失败，执行类不存在" });
        }

        _context.ScheduledJobs.Attach(scheduledjob);
        _context.Entry(scheduledjob).Property("JobName").IsModified = true;
        _context.Entry(scheduledjob).Property("JobGroup").IsModified = true;
        _context.Entry(scheduledjob).Property("CronExpression").IsModified = true;
        _context.Entry(scheduledjob).Property("Data").IsModified = true;
        _context.Entry(scheduledjob).Property("Status").IsModified = true;

        var isSuccess = await _context.SaveChangesAsync(cancellationToken) > 0;
        if (isSuccess) 
        {
            var triggerKey = new TriggerKey(scheduledjob.TriggerName!, scheduledjob.TriggerGroup!);
            var jobKey = new JobKey(scheduledjob.JobName!, scheduledjob.JobGroup);

            if (await _quartzService.CheckExistsAsync(oldJobKey))
            {
                var jobStatus = await _quartzService.GetJobStateAsync(oldTriggerKey);
                if (jobStatus == TriggerState.Normal)
                {
                    await _quartzService.PauseJobAsync(oldJobKey, cancellationToken);
                    await _quartzService.UnscheduleJobAsync(oldTriggerKey);
                }

                await _quartzService.DeleteJobAsync(oldJobKey, cancellationToken);
            }
            if (scheduledjob.Status.HasValue && scheduledjob.Status.Value == JobStatus.Normal)
            {
                var trigger = TriggerBuilder.Create()
                   .WithIdentity(triggerKey)
                   .WithCronSchedule(scheduledjob.CronExpression!)
                   .Build();

                var type = assembly.GetType(scheduledjob.JobGroup!)!;
                var job = JobBuilder.Create(type)
                    .WithIdentity(scheduledjob.JobName!, scheduledjob.JobGroup!)
                    .UsingJobData("parameter", request.Data)
                    .Build();

                await _quartzService.AddJobAsync(job, trigger);
            }
            return await Result<long>.SuccessAsync(scheduledjob.Id);
        }
        return await Result<long>.FailureAsync(new string[] { "操作失败" });
    }

    /// <summary>
    /// 业务逻辑
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Result<long>> Handle(UpdateScheduledJobStatusCommand request, CancellationToken cancellationToken)
    {
        var scheduledjob = await _context.ScheduledJobs
           .SingleOrDefaultAsync(x => x.Id == request.ScheduledJobId, cancellationToken)
           ?? throw new NotFoundException($"数据【{request.ScheduledJobId}】未找到");

        var jobKey = new JobKey(scheduledjob.JobName!, scheduledjob.JobGroup);
        var triggerKey = new TriggerKey(scheduledjob.TriggerName!, scheduledjob.TriggerGroup!);

        scheduledjob = _mapper.Map(request, scheduledjob);
        Assembly assembly = Assembly.Load(scheduledjob.JobGroup!.Split('.')[0].ToString());

        if (assembly == null)
        {
            return await Result<long>.SuccessOrFailureAsync(scheduledjob.Id, false, new string[] { "操作失败，执行类不存在" });
        }

        _context.ScheduledJobs.Attach(scheduledjob);
        _context.Entry(scheduledjob).Property("Status").IsModified = true;
        var isSuccess = await _context.SaveChangesAsync(cancellationToken) > 0;
        if (isSuccess)
        {
            if (await _quartzService.CheckExistsAsync(jobKey))
            {
                var jobStatus = await _quartzService.GetJobStateAsync(triggerKey);
                if (jobStatus == TriggerState.Normal) await _quartzService.PauseJobAsync(jobKey, cancellationToken);
                else if (jobStatus == TriggerState.Paused) await _quartzService.ResumeJobAsync(jobKey, cancellationToken);
                else
                {
                    await _quartzService.DeleteJobAsync(jobKey, cancellationToken);

                    if (scheduledjob.Status == JobStatus.Normal)
                    {
                        var trigger = TriggerBuilder.Create()
                            .WithIdentity(triggerKey)
                            .WithCronSchedule(scheduledjob.CronExpression!)
                            .Build();

                        var type = assembly.GetType(scheduledjob.JobGroup)!;
                        var job = JobBuilder.Create(type)
                            .WithIdentity(jobKey)
                            .UsingJobData("parameter", scheduledjob.Data)
                            .Build();

                        await _quartzService.AddJobAsync(job, trigger);
                    }
                }
            }
            else 
            {
                if (scheduledjob.Status == JobStatus.Normal)
                {
                    var trigger = TriggerBuilder.Create()
                       .WithIdentity(triggerKey)
                       .WithCronSchedule(scheduledjob.CronExpression!)
                       .Build();

                    var type = assembly.GetType(scheduledjob.JobGroup)!;
                    var job = JobBuilder.Create(type)
                        .WithIdentity(jobKey)
                        .UsingJobData("parameter", scheduledjob.Data)
                        .Build();

                    await _quartzService.AddJobAsync(job, trigger);
                }
            }
            return await Result<long>.SuccessAsync(scheduledjob.Id);
        }
        return await Result<long>.FailureAsync(new string[] { "操作失败" });
    }
}
