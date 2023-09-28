using Application.Features.TestTables.DTOs;
using Application.Features.TestTables.Commands.Add;
using Application.Features.TestTables.Commands.Delete;
using Application.Features.TestTables.Commands.Update;
using Application.Features.TestTables.Queries.Pagination;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

/// <summary>
/// 测试表
/// </summary>
public class TestTableController : ApiControllerBase
{
    /// <summary>
    /// 分页查询
    /// </summary>
    /// <returns></returns>
    [HttpPost("PaginationQuery")]

    public async Task<Result<PaginatedData<TestTableDto>>> PaginationQuery(TestTablesWithPaginationQuery query)
    {
        return await Mediator.Send(query);
    }

    /// <summary>
    /// 创建测试表
    /// </summary>
    /// <returns></returns>
    [HttpPost("Add")]

    public async Task<Result<long>> Add(AddTestTableCommand command)
    {
        return await Mediator.Send(command);
    }

    /// <summary>
    /// 修改测试表
    /// </summary>
    /// <returns></returns>
    [HttpPut("Update")]
    public async Task<Result<long>> Update(UpdateTestTableCommand command)
    {
        return await Mediator.Send(command);
    }

    /// <summary>
    /// 删除测试表
    /// </summary>
    /// <returns></returns>
    [HttpDelete("Delete")]
    public async Task<Result<bool>> Delete(DeleteTestTableCommand command)
    {
        return await Mediator.Send(command);
    }
}
