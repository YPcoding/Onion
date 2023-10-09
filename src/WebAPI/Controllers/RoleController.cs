using Application.Features.Roles.Commands.Add;
using Application.Features.Roles.Commands.Delete;
using Application.Features.Roles.Commands.Update;
using Application.Features.Roles.DTOs;
using Application.Features.Roles.Queries.GetAll;
using Application.Features.Roles.Queries.GetById;
using Application.Features.Roles.Queries.GetMenusById;
using Application.Features.Roles.Queries.Pagination;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace WebAPI.Controllers;

/// <summary>
/// 角色管理
/// </summary>
[Description("角色管理")]
public class RoleController : ApiControllerBase
{
    /// <summary>
    /// 分页查询
    /// </summary>
    /// <returns></returns>
    [HttpPost("PaginationQuery")]

    public async Task<Result<PaginatedData<RoleDto>>> PaginationQuery(RolesWithPaginationQuery query)
    {
        return await Mediator.Send(query);
    }

    /// <summary>
    /// 单个查询
    /// </summary>
    /// <param name="roleId">角色唯一标识</param>
    /// <returns></returns>
    [HttpGet("Query/{roleId}")]

    public async Task<Result<RoleDto>> GetByIdQuery(long roleId)
    {
        return await Mediator.Send(new GetRoleQueryById { RoleId = roleId });
    }

    /// <summary>
    /// 获取所有角色
    /// </summary>
    /// <returns></returns>
    [HttpGet("Query/All")]
    public async Task<Result<IEnumerable<RoleDto>>>  GetAllRoleQuery()
    {
        return await Mediator.Send(new GetAllRolesQuery());
    }

    /// <summary>
    /// 获取用户角色
    /// </summary>
    /// <returns></returns>
    [HttpGet("Query/All/By/{userId}")]
    public async Task<Result<IEnumerable<RoleDto>>> GetAllRoleQueryByUserId(long userId)
    {
        return await Mediator.Send(new GetAllRoleQueryByUserId() { UserId = userId });
    }

    /// <summary>
    /// 获取角色菜单
    /// </summary>
    /// <returns></returns>
    [HttpGet("Query/Menu/By/{roleId}")]
    public async Task<Result<IEnumerable<RoleMenuDto>>> GetRoleMenuQueryById(long roleId)
    {
        return await Mediator.Send(new GetRoleMenuQueryById() { RoleId = roleId });
    }

    /// <summary>
    /// 创建角色
    /// </summary>
    /// <returns></returns>
    [HttpPost("Add")]

    public async Task<Result<long>> Add(AddRoleCommand command)
    {
        return await Mediator.Send(command);
    }

    /// <summary>
    /// 修改角色
    /// </summary>
    /// <returns></returns>
    [HttpPut("Update")]

    public async Task<Result<long>> Update(UpdateRoleCommand command)
    {
        return await Mediator.Send(command);
    }

    /// <summary>
    /// 修改角色是否激活
    /// </summary>
    /// <returns></returns>
    [HttpPut("IsActive")]

    public async Task<Result<bool>> IsActive(UpdatedRoleIsActiveCommand command)
    {
        return await Mediator.Send(command);
    }


    /// <summary>
    /// 修改角色菜单权限
    /// </summary>
    /// <returns></returns>
    [HttpPut("Menu")]
    public async Task<Result<bool>> UpdateRoleMenu(UpdateRoleMenuCommand command)
    {
        return await Mediator.Send(command);
    }

    /// <summary>
    /// 删除角色
    /// </summary>
    /// <returns></returns>
    [HttpDelete("Delete")]

    public async Task<Result<bool>> Delete(DeleteRoleCommand command)
    {
        return await Mediator.Send(command);
    }
}
