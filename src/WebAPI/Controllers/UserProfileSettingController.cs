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

namespace WebAPI.Controllers;

/// <summary>
/// 个人设置
/// </summary>
public class UserProfileSettingController : ApiControllerBase
{
    private readonly ICurrentUserService _currentUserService;

    public UserProfileSettingController(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
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
