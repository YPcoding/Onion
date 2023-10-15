using Microsoft.AspNetCore.SignalR;
namespace Application.Hubs;

public class BuildSendMessage 
{
    public BuildSendMessage(HubCallerContext context, string message)
    {
        Message = message.FromJson<Message>();

        Message.Sender.ProfilePictureDataUrl = context.User?.Claims?.FirstOrDefault(c => c.Type == "ProfilePictureDataUrl")?.Value;
        Message.Sender.UserId = context.User?.Claims?.FirstOrDefault()?.Value;
        Message.Sender.UserName = context.User?.Identity?.Name;
    }
    public Message Message { get; set; } = new Message();
}

public class Message 
{
    public string Type { get { return "incoming"; } set { } }
    public string Content { get; set; }
    public string ContentType { get; set; }
    public string Timestamp { get; set; }
    public Sender Sender { get; set; } = new Sender();
}

public class Sender
{
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string? ProfilePictureDataUrl { get; set; }
}