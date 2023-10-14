using Application.Features.ScheduledJobs.DTOs;
using Application.Features.ScheduledJobs.Commands.Add;
using Application.Features.ScheduledJobs.Commands.Delete;
using Application.Features.ScheduledJobs.Commands.Update;
using Application.Features.ScheduledJobs.Queries.Pagination;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace WebAPI.Controllers;

/// <summary>
/// 计划任务
/// </summary>
[Description("计划任务")]
public class ScheduledJobController : ApiControllerBase
{
    /// <summary>
    /// 分页查询
    /// </summary>
    /// <returns></returns>
    [HttpPost("PaginationQuery")]
    public async Task<Result<PaginatedData<ScheduledJobDto>>> PaginationQuery(ScheduledJobsWithPaginationQuery query)
    {
        return await Mediator.Send(query);
    }

    /// <summary>
    /// 任务日志分页查询
    /// </summary>
    /// <returns></returns>
    [HttpPost("Log/PaginationQuery")]

    public async Task<Result<PaginatedData<ScheduledJobLogDto>>> LogPaginationQuery(ScheduledJobLogsWithPaginationQuery query)
    {
        return await Mediator.Send(query);
    }

    /// <summary>
    /// 获取执行任务分组类名称
    /// </summary>
    /// <returns></returns>
    [HttpGet("JobGroup/Query")]

    public async Task<Result<IEnumerable<JobGroupDto>>> JobGroupQuery()
    {
        return await Mediator.Send(new ScheduledJobGroupQuery());
    }

    /// <summary>
    /// 创建定时任务
    /// </summary>
    /// <returns></returns>
    [HttpPost("Add")]

    public async Task<Result<long>> Add(AddScheduledJobCommand command)
    {
        return await Mediator.Send(command);
    }

    /// <summary>
    /// 修改任务状态
    /// </summary>
    /// <returns></returns>
    [HttpPut("Update/JobStatus")]
    public async Task<Result<long>> UpdateJobStatus(UpdateScheduledJobStatusCommand command)
    {
        return await Mediator.Send(command);
    }

    /// <summary>
    /// 修改定时任务
    /// </summary>
    /// <returns></returns>
    [HttpPut("Update")]
    public async Task<Result<long>> Update(UpdateScheduledJobCommand command)
    {
        return await Mediator.Send(command);
    }

    /// <summary>
    /// 删除定时任务
    /// </summary>
    /// <returns></returns>
    [HttpDelete("Delete")]
    public async Task<Result<bool>> Delete(DeleteScheduledJobCommand command)
    {
        return await Mediator.Send(command);
    }
}
