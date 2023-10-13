using Common.Quartzs;
using Domain.Entities.Job;
using Quartz;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Application.Features.ScheduledJobs.Commands.Add;

/// <summary>
/// 添加定时任务
/// </summary>
[Map(typeof(ScheduledJob))]
public class AddScheduledJobCommand : IRequest<Result<long>>
{
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
/// 处理程序
/// </summary>
public class AddScheduledJobCommandHandler : IRequestHandler<AddScheduledJobCommand, Result<long>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IQuartzService _quartzService;
    public AddScheduledJobCommandHandler(
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
    public async Task<Result<long>> Handle(AddScheduledJobCommand request, CancellationToken cancellationToken)
    {
        var scheduledjob = _mapper.Map<ScheduledJob>(request);
        scheduledjob.TriggerName = scheduledjob.JobName;
        scheduledjob.TriggerGroup = scheduledjob.JobGroup;

        await _context.ScheduledJobs.AddAsync(scheduledjob);
        var isSuccess = await _context.SaveChangesAsync(cancellationToken) > 0;
        if (isSuccess)
        {        
            if (scheduledjob.Status.HasValue && scheduledjob.Status == JobStatus.Normal) 
            {
                var trigger = TriggerBuilder.Create()
                    .WithIdentity(scheduledjob.TriggerName!, scheduledjob.TriggerGroup!)
                    .WithCronSchedule(scheduledjob.CronExpression!)
                    .Build();

                Assembly assembly = Assembly.Load(scheduledjob.JobGroup!.Split('.')[0].ToString());
                var type = assembly.GetType(scheduledjob.JobGroup)!;
                var job = JobBuilder.Create(type)
                    .WithIdentity(scheduledjob.JobName!, scheduledjob.JobGroup)
                    .UsingJobData("parameter", request.Data)
                    .Build();

                await _quartzService.AddJobAsync(job, trigger);
            }

            return await Result<long>.SuccessAsync(scheduledjob.Id);
        }
        return await Result<long>.FailureAsync(new string[] { "操作失败" });
    }
}