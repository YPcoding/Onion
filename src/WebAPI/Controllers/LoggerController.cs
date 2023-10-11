using Application.Features.Loggers.DTOs;
using Application.Features.Loggers.Queries.Pagination;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace WebAPI.Controllers;

/// <summary>
/// 系统日志
/// </summary>
[Description("系统日志")]
public class LoggerController : ApiControllerBase
{
    /// <summary>
    /// 分页查询
    /// </summary>
    /// <returns></returns>
    [HttpPost("PaginationQuery")]

    public async Task<Result<PaginatedData<LoggerDto>>> PaginationQuery(SystemLoggersWithPaginationQuery query)
    {
        return await Mediator.Send(query);
    }
}
