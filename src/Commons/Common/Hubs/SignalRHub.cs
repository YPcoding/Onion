using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace Application.Common.Interfaces;

/// <summary>
/// SignalR Hub 用于处理聊天相关操作。
/// </summary>
public class SignalRHub : Hub
{
    private static readonly ConcurrentDictionary<string, string> OnlineUsers = new();

    public async Task SendNotification(string message)
    {
        // 客户端方法，接收消息
        await Clients.All.SendAsync("ReceiveNotification", message);
    }

    /// <summary>
    /// 向指定用户发送私人消息。
    /// </summary>
    public async Task SendPrivateMessageAsync(string user, string message)
    {
        // 向指定用户发送消息
        await Clients.User(user).SendAsync("ReceivePrivateMessage", message);
    }

    /// <summary>
    /// 向指定群组发送群组消息。
    /// </summary>
    public async Task SendGroupMessageAsync(string groupName, string user, string message)
    {
        // 向指定群组发送消息
        await Clients.Group(groupName).SendAsync("ReceiveGroupMessage", user, message);
    }

    /// <summary>
    /// 向所有连接的客户端发送公共消息。
    /// </summary>
    public async Task SendPublicMessageAsync(string user, string message)
    {
        // 向所有连接的客户端发送消息
        await Clients.AllExcept(Context.ConnectionId).SendAsync("ReceivePublicMessage", user, message);
    }

    /// <summary>
    /// 加入指定聊天组。
    /// </summary>
    public async Task JoinGroupAsync(string groupName)
    {
        // 加入指定聊天组
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    }

    /// <summary>
    /// 离开指定聊天组。
    /// </summary>
    public async Task LeaveGroupAsync(string groupName)
    {
        // 离开指定聊天组
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
    }

    /// <summary>
    /// 客户端连接时的逻辑。
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        var connectionId = Context.ConnectionId;
        var userName = Context.User?.Identity?.Name ?? string.Empty;
        if (!OnlineUsers.ContainsKey(connectionId)) OnlineUsers.TryAdd(connectionId, userName);

        await SendPublicMessageAsync(userName,"上线");

        // 客户端连接时的逻辑
        await base.OnConnectedAsync();
    }

    /// <summary>
    /// 客户端断开连接时的逻辑。
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception exception)
    {
        // 客户端断开连接时的逻辑
        await base.OnDisconnectedAsync(exception);
    }
}
