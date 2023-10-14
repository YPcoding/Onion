using Application.Features.Chats.DTOs;
using Application.Features.Chats.Queries;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

public class ChatController : ApiControllerBase
{
    /// <summary>
    /// 聊天用户分页查询
    /// </summary>
    /// <returns></returns>
    [HttpPost("PaginationQuery")]

    public async Task<Result<PaginatedData<ChatUserDto>>> PaginationQuery(ChatUsersWithPaginationQuery query)
    {
        return await Mediator.Send(query);
    }
}
