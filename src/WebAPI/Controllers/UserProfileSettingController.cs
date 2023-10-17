using Application.Features.UserProfileSettings.DTOs;
using Application.Features.UserProfileSettings.Commands.Add;
using Application.Features.UserProfileSettings.Commands.Delete;
using Application.Features.UserProfileSettings.Commands.Update;
using Application.Features.UserProfileSettings.Queries.Pagination;
using Microsoft.AspNetCore.Mvc;
using Application.Features.UserProfileSettings.Commands.Save;
using Application.Features.UserProfileSettings.Queries.GetAll;
using Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel;
using Application.Features.Users.Commands.Update;
using Application.Features.Users.DTOs;
using Application.Features.Users.Queries.GetById;
using Application.Features.Loggers.DTOs;
using Application.Features.Loggers.Queries.Pagination;
using Application.Constants.Loggers;

namespace WebAPI.Controllers;

/// <summary>
/// 帐号信息
/// </summary>
[Description("帐号信息")]
public class UserProfileSettingController : ApiControllerBase
{
    private readonly ICurrentUserService _currentUserService;

    public UserProfileSettingController(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// 个人操作日志
    /// </summary>
    /// <returns></returns>
    [HttpPost("Log/PaginationQuery")]

    public async Task<Result<PaginatedData<LoggerDto>>> LogPaginationQuery(LoggersWithPaginationQuery query)
    {
        query.Template = MessageTemplate.ActivityHistoryLog;
        query.Message = _currentUserService.UserName;
        return await Mediator.Send(query);
    }


    /// <summary>
    /// 分页查询
    /// </summary>
    /// <returns></returns>
    [HttpPost("PaginationQuery")]
    [AllowAnonymous]
    public async Task<Result<PaginatedData<UserProfileSettingsDto>>> PaginationQuery(UserProfileSettingsWithPaginationQuery query)
    {
        return await Mediator.Send(query);
    }

    /// <summary>
    /// 获取个人信息
    /// </summary>
    /// <returns></returns>
    [HttpGet("Info/Query")]
    public async Task<Result<UserDto>> GetUserInfoQuery()
    {
        return await Mediator.Send(new GetUserByIdQuery { UserId = _currentUserService.CurrentUserId });
    }

    /// <summary>
    /// 修改个人信息
    /// </summary>
    /// <returns></returns>
    [HttpPut("Update/Info")]
    public async Task<Result<long>> UpdateUserInfo(UpdateUserInfoCommand command)
    {
        return await Mediator.Send(command);
    }


    /// <summary>
    /// 更改密码
    /// </summary>
    /// <returns></returns>
    [HttpPut("Change/Password")]
    public async Task<Result<bool>> ChangePassword(ChangePasswordCommand command)
    {
        return await Mediator.Send(command);
    }

    /// <summary>
    /// 获取个人设置
    /// </summary>
    /// <returns></returns>
    [HttpGet("Query")]
    [AllowAnonymous]
    public async Task<Result<IEnumerable<UserProfileSettingsDto>>> GetUserProfileSettingsQueryByUserId()
    {
        return await Mediator.Send(new GetUserProfileSettingsQueryByUserId() { UserId = _currentUserService.CurrentUserId });
    }

    /// <summary>
    /// 创建个人设置
    /// </summary>
    /// <returns></returns>
    [HttpPost("Add")]

    public async Task<Result<long>> Add(AddUserProfileSettingsCommand command)
    {
        return await Mediator.Send(command);
    }

    /// <summary>
    /// 保存个人设置
    /// </summary>
    /// <returns></returns>
    [HttpPost("Save")]

    public async Task<Result<bool>> Save(SaveUserProfileSettingsCommand command)
    {
        return await Mediator.Send(command);
    }

    /// <summary>
    /// 修改个人设置
    /// </summary>
    /// <returns></returns>
    [HttpPut("Update")]
    public async Task<Result<long>> Update(UpdateUserProfileSettingsCommand command)
    {
        return await Mediator.Send(command);
    }

    /// <summary>
    /// 删除个人设置
    /// </summary>
    /// <returns></returns>
    [HttpDelete("Delete")]
    public async Task<Result<bool>> Delete(DeleteUserProfileSettingsCommand command)
    {
        return await Mediator.Send(command);
    }
}
