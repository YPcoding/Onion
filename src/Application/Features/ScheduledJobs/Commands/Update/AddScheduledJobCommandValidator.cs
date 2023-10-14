namespace Application.Features.ScheduledJobs.Commands.Update;

public class UpdateScheduledJobCommandValidator : AbstractValidator<UpdateScheduledJobCommand>
{
    private readonly IApplicationDbContext _context;    
    public UpdateScheduledJobCommandValidator(IApplicationDbContext context)
    {
        _context = context;
        RuleFor(v => v.JobName)
            .NotEmpty().WithMessage("任务名称不能为空")
            .MustAsync(BeUniqueJobKeyAsync).WithMessage($"任务名称重复，请修改");

        RuleFor(v => v.JobGroup)
            .NotEmpty().WithMessage("任务执行类不能为空");
    }

    /// <summary>
    /// 校验任务键是否唯一
    /// </summary>
    /// <param name="command">参数</param>
    ///  <param name="jobName">任务名称</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task<bool> BeUniqueJobKeyAsync(UpdateScheduledJobCommand command,string jobName, CancellationToken cancellationToken)
    {
        var jop = await _context.ScheduledJobs.FirstOrDefaultAsync(x => x.JobName == jobName && x.JobGroup == command.JobGroup, cancellationToken);
        if (jop == null)
        {
            return true;
        }
        return jop.Id == command.ScheduledJobId;
    }
}