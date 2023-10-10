using Application.Features.Loggers.DTOs;
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
}
