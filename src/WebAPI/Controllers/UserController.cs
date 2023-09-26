﻿using Application.Features.Roles.Commands.Update;
using Application.Features.Users.Commands.Add;
using Application.Features.Users.Commands.Delete;
using Application.Features.Users.Commands.Update;
using Application.Features.Users.DTOs;
using Application.Features.Users.Queries.GetAll;
using Application.Features.Users.Queries.GetById;
using Application.Features.Users.Queries.Pagination;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

/// <summary>
/// 用户管理
/// </summary>
public class UserController : ApiControllerBase
{
    /// <summary>
    /// 分页查询
    /// </summary>
    /// <returns></returns>
    [HttpPost("PaginationQuery")]

    public async Task<Result<PaginatedData<UserDto>>> PaginationQuery(UsersWithPaginationQuery query)
    {
        return await Mediator.Send(query);
    }

    /// <summary>
    /// 单个查询
    /// </summary>
    /// <param name="userId">用户唯一标识</param>
    /// <returns></returns>
    [HttpGet("Query/{userId}")]

    public async Task<Result<UserDto>> GetByIdQuery(long userId)
    {
        return await Mediator.Send(new GetUserByIdQuery { UserId = userId });
    }

    /// <summary>
    /// 分配角色
    /// </summary>
    /// <returns></returns>
    [HttpPut("Assigning/Role")]
    public async Task<Result<bool>> Assigning(AssigningRoleCommand command)
    {
        return await Mediator.Send(command);
    }

    /// <summary>
    /// 锁定或解锁用户
    /// </summary>
    /// <returns></returns>
    [HttpPut("IsUnLock")]
    public async Task<Result<bool>> Is(IsUnLockUserCommand command)
    {
        return await Mediator.Send(command);
    }

    /// <summary>
    /// 获取所有用户
    /// </summary>
    /// <returns></returns>
    [HttpGet("Query/All")]
    public async Task<Result<IEnumerable<UserDto>>> GetAllUsersQuery()
    {
        return await Mediator.Send(new GetAllUsersQuery());
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
    /// 修改用户头像
    /// </summary>
    /// <returns></returns>
    [HttpPut("Update/Avatar")]
    public async Task<Result<long>> UpdateAvatar(UpdateUserAvatarCommand command)
    {
        return await Mediator.Send(command);
    }

    /// <summary>
    /// 重置密码
    /// </summary>
    /// <returns></returns>
    [HttpPut("Reset/Password")]
    public async Task<Result<long>> ResetPassword(ResetPasswordCommand command)
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
