using Application.Features.Notifications.DTOs;
using Application.Features.Notifications.Commands.Add;
using Application.Features.Notifications.Commands.Delete;
using Application.Features.Notifications.Commands.Update;
using Application.Features.Notifications.Queries.Pagination;
using Domain.Entities.Notifications;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

/// <summary>
/// 通知
/// </summary>
public class NotificationController : ApiControllerBase
{
    /// <summary>
    /// 分页查询
    /// </summary>
    /// <returns></returns>
    [HttpPost("PaginationQuery")]

    public async Task<Result<PaginatedData<NotificationDto>>> PaginationQuery(NotificationsWithPaginationQuery query)
    {
        return await Mediator.Send(query);
    }

    /// <summary>
    /// 创建通知
    /// </summary>
    /// <returns></returns>
    [HttpPost("Add")]

    public async Task<Result<long>> Add(AddNotificationCommand command)
    {
        return await Mediator.Send(command);
    }

    /// <summary>
    /// 修改通知
    /// </summary>
    /// <returns></returns>
    [HttpPut("Update")]
    public async Task<Result<long>> Update(UpdateNotificationCommand command)
    {
        return await Mediator.Send(command);
    }

    /// <summary>
    /// 删除通知
    /// </summary>
    /// <returns></returns>
    [HttpDelete("Delete")]
    public async Task<Result<bool>> Delete(DeleteNotificationCommand command)
    {
        return await Mediator.Send(command);
    }
}
