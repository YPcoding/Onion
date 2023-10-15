using Application.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace Application.Common;

/// <summary>
/// SignalR Hub 用于处理聊天相关操作。
/// </summary>
public class SignalRHub : Hub
{
    private static readonly ConcurrentDictionary<string, string> OnlineUsers = new();

    /// <summary>
    /// 向所有客户发送一条通知
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public async Task SendNotificationAsync(string message)
    {
        // 客户端方法，接收消息
        await Clients.All.SendAsync("ReceiveNotification", message);
    }

    /// <summary>
    /// 向指定用户发送私人消息。
    /// </summary>
    /// <param name="userId">目标用户ID</param>
    /// <param name="message">要发送的消息</param>
    public async Task SendPrivateMessageAsync(string userId, string message)
    {
        var sendMessage = new BuildSendMessage(Context, message);
        // 向指定用户发送消息
        await Clients.User(userId).SendAsync("ReceivePrivateMessage", sendMessage.Message.ToJson(true));
    }

    /// <summary>
    /// 向指定群组发送群组消息。
    /// </summary>
    /// <param name="groupName">目标群组名称</param>
    /// <param name="user">发送消息的用户</param>
    /// <param name="message">要发送的消息</param>
    public async Task SendGroupMessageAsync(string groupName, string user, string message)
    {
        // 向指定群组发送消息
        await Clients.Group(groupName).SendAsync("ReceiveGroupMessage", user, message);
    }

    /// <summary>
    /// 向所有连接的客户端发送公共消息。
    /// </summary>
    /// <param name="user">发送消息的用户</param>
    /// <param name="message">要发送的消息</param>
    public async Task SendPublicMessageAsync(string user, string message)
    {
        // 向所有连接的客户端发送消息
        await Clients.AllExcept(Context.ConnectionId).SendAsync("ReceivePublicMessage", user, message);
    }


    /// <summary>
    /// 加入指定聊天组。
    /// </summary>
    /// <param name="groupName">聊天组名称</param>
    public async Task JoinGroupAsync(string groupName)
    {
        // 加入指定聊天组
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    }

    /// <summary>
    /// 离开指定聊天组。
    /// </summary>
    /// <param name="groupName">聊天组名称</param>
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

        await SendPublicMessageAsync(userName, "上线");

        // 客户端连接时的逻辑
        await base.OnConnectedAsync();
    }

    /// <summary>
    /// 客户端断开连接时的逻辑。
    /// </summary>
    /// <param name="exception">断开连接时可能发生的异常</param>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var connectionId = Context.ConnectionId;
        if (OnlineUsers.TryRemove(connectionId, out var userName)) await SendPublicMessageAsync(userName, "下线");

        // 客户端断开连接时的逻辑
        await base.OnDisconnectedAsync(exception);
    }
}