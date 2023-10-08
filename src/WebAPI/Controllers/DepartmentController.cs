using Application.Features.Departments.DTOs;
using Application.Features.Departments.Commands.Add;
using Application.Features.Departments.Commands.Delete;
using Application.Features.Departments.Commands.Update;
using Application.Features.Departments.Queries.Pagination;
using Microsoft.AspNetCore.Mvc;
using Application.Features.Departments.Queries.GetAll;
using System.ComponentModel;

namespace WebAPI.Controllers;

/// <summary>
/// 部门管理
/// </summary>
[Description("部门管理")]
public class DepartmentController : ApiControllerBase
{
    /// <summary>
    /// 查询全部
    /// </summary>
    /// <returns></returns>
    [HttpPost("Query/All")]

    public async Task<Result<IEnumerable<DepartmentDto>>> All(GetAllDepartmentsQuery query)
    {
        return await Mediator.Send(query);
    }

    /// <summary>
    /// 分页查询
    /// </summary>
    /// <returns></returns>
    [HttpPost("PaginationQuery")]

    public async Task<Result<PaginatedData<DepartmentDto>>> PaginationQuery(DepartmentsWithPaginationQuery query)
    {
        return await Mediator.Send(query);
    }

    /// <summary>
    /// 创建
    /// </summary>
    /// <returns></returns>
    [HttpPost("Add")]

    public async Task<Result<long>> Add(AddDepartmentCommand command)
    {
        return await Mediator.Send(command);
    }

    /// <summary>
    /// 修改
    /// </summary>
    /// <returns></returns>
    [HttpPut("Update")]
    public async Task<Result<long>> Update(UpdateDepartmentCommand command)
    {
        return await Mediator.Send(command);
    }

    /// <summary>
    /// 删除
    /// </summary>
    /// <returns></returns>
    [HttpDelete("Delete")]
    public async Task<Result<bool>> Delete(DeleteDepartmentCommand command)
    {
        return await Mediator.Send(command);
    }
}
