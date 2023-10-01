using Application.Features.AuditTrails.DTOs;
using Application.Features.AuditTrails.Commands.Add;
using Application.Features.AuditTrails.Commands.Delete;
using Application.Features.AuditTrails.Commands.Update;
using Application.Features.AuditTrails.Queries.Pagination;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace WebAPI.Controllers;

/// <summary>
/// 审计日志
/// </summary>
[Description("日志管理")]
public class AuditTrailController : ApiControllerBase
{
    /// <summary>
    /// 分页查询
    /// </summary>
    /// <returns></returns>
    [HttpPost("PaginationQuery")]

    public async Task<Result<PaginatedData<AuditTrailDto>>> PaginationQuery(AuditTrailsWithPaginationQuery query)
    {
        return await Mediator.Send(query);
    }

    /// <summary>
    /// 创建审计日志
    /// </summary>
    /// <returns></returns>
    [HttpPost("Add")]

    public async Task<Result<long>> Add(AddAuditTrailCommand command)
    {
        return await Mediator.Send(command);
    }

    /// <summary>
    /// 修改审计日志
    /// </summary>
    /// <returns></returns>
    [HttpPut("Update")]
    public async Task<Result<long>> Update(UpdateAuditTrailCommand command)
    {
        return await Mediator.Send(command);
    }

    /// <summary>
    /// 删除审计日志
    /// </summary>
    /// <returns></returns>
    [HttpDelete("Delete")]
    public async Task<Result<bool>> Delete(DeleteAuditTrailCommand command)
    {
        return await Mediator.Send(command);
    }
}
