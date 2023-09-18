using Application.Features.Users.Commands.AddEdit;
using Application.Features.Users.Commands.Delete;
using Application.Features.Users.Commands.Update;
using Application.Features.Users.DTOs;
using Application.Features.Users.Queries.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

/// <summary>
/// 用户管理
/// </summary>
public class UserController : ApiControllerBase
{
    /// <summary>
    /// 用户分页查询
    /// </summary>
    /// <returns></returns>
    [HttpPost("PaginationQuery")]
    [AllowAnonymous]

    public async Task<Result<PaginatedData<UserDto>>> PaginationQuery(UsersWithPaginationQuery query)
    {
        return await Mediator.Send(query);
    }

    /// <summary>
    /// 创建用户
    /// </summary>
    /// <returns></returns>
    [HttpPost("Add")]

    public async Task<Result<long>> Add(AddUserCommand command)
    {
        return await Mediator.Send(command);
    }

    /// <summary>
    /// 修改用户
    /// </summary>
    /// <returns></returns>
    [HttpPut("Update")]
    public async Task<Result<long>> Update(UpdateUserCommand command)
    {
        return await Mediator.Send(command);
    }

    /// <summary>
    /// 删除用户
    /// </summary>
    /// <returns></returns>
    [HttpDelete("Delete")]

    public async Task<Result<bool>> Delete(DeleteUserCommand command)
    {
        return await Mediator.Send(command);
    }
}
