using Application.Features.Auth.Commands;
using Common.Quartzs;
using Domain.Entities.Job;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Quartz;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Application.Features.ScheduledJobs.Commands.Add;

public class AddScheduledJobCommandValidator : AbstractValidator<AddScheduledJobCommand>
{
    private readonly IApplicationDbContext _context;    
    public AddScheduledJobCommandValidator(IApplicationDbContext context)
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
    private async Task<bool> BeUniqueJobKeyAsync(AddScheduledJobCommand command,string jobName, CancellationToken cancellationToken)
    {
        return !(await _context.ScheduledJobs.AnyAsync(x => x.JobName == jobName && x.JobGroup == command.JobGroup, cancellationToken));
    }
}