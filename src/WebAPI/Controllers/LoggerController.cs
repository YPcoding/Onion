using Application.Features.Loggers.DTOs;
using Application.Features.Loggers.Commands.Add;
using Application.Features.Loggers.Commands.Delete;
using Application.Features.Loggers.Commands.Update;
using Application.Features.Loggers.Queries.Pagination;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

/// <summary>
/// 日志
/// </summary>
public class LoggerController : ApiControllerBase
{
    /// <summary>
    /// 分页查询
    /// </summary>
    /// <returns></returns>
    [HttpPost("PaginationQuery")]

    public async Task<Result<PaginatedData<LoggerDto>>> PaginationQuery(LoggersWithPaginationQuery query)
    {
        return await Mediator.Send(query);
    }

    /// <summary>
    /// 创建日志
    /// </summary>
    /// <returns></returns>
    [HttpPost("Add")]

    public async Task<Result<long>> Add(AddLoggerCommand command)
    {
        return await Mediator.Send(command);
    }

    /// <summary>
    /// 修改日志
    /// </summary>
    /// <returns></returns>
    [HttpPut("Update")]
    public async Task<Result<long>> Update(UpdateLoggerCommand command)
    {
        return await Mediator.Send(command);
    }

    /// <summary>
    /// 删除日志
    /// </summary>
    /// <returns></returns>
    [HttpDelete("Delete")]
    public async Task<Result<bool>> Delete(DeleteLoggerCommand command)
    {
        return await Mediator.Send(command);
    }
}
