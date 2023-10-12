using Common.Quartzs;
using Quartz;

namespace Application.Features.ScheduledJobs.Commands.Delete;

/// <summary>
/// 删除定时任务
/// </summary>
public class DeleteScheduledJobCommand : IRequest<Result<bool>>
{
  
        /// <summary>
        /// 唯一标识
        /// </summary>
        [Description("唯一标识")]
        public List<long> ScheduledJobIds { get; set; }
}

/// <summary>
/// 处理程序
/// </summary>
public class DeleteScheduledJobCommandHandler : IRequestHandler<DeleteScheduledJobCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly IQuartzService _quartzService;

    public DeleteScheduledJobCommandHandler(
        IApplicationDbContext context,
        IQuartzService quartzService)
    {
        _context = context;
        _quartzService = quartzService;
    }

    /// <summary>
    /// 业务逻辑
    /// </summary>
    /// <param name="request">请求参数</param>
    /// <param name="cancellationToken">取消标记</param>
    /// <returns>返回处理结果</returns>
    public async Task<Result<bool>> Handle(DeleteScheduledJobCommand request, CancellationToken cancellationToken)
    {
        var scheduledjobsToDelete = await _context.ScheduledJobs
            .Where(x => request.ScheduledJobIds.Contains(x.Id))
            .ToListAsync();

        if (scheduledjobsToDelete.Any())
        {
            _context.ScheduledJobs.RemoveRange(scheduledjobsToDelete);
            var isSuccess = await _context.SaveChangesAsync(cancellationToken) > 0;
            if (isSuccess) 
            {
                foreach (var scheduledjob in scheduledjobsToDelete) 
                {
                    var jobKey = new JobKey(scheduledjob.JobName!, scheduledjob.JobGroup);
                    if (await _quartzService.CheckExistsAsync(jobKey))
                    {
                        await _quartzService.PauseJobAsync(jobKey, cancellationToken);
                        await _quartzService.DeleteJobAsync(jobKey, cancellationToken);
                    }
                }
            }
            return await Result<bool>.SuccessOrFailureAsync(isSuccess, isSuccess,new string[] {"操作失败"});
        }

        return await Result<bool>.FailureAsync(new string[] { "没有找到需要删除的数据" });
    }
}