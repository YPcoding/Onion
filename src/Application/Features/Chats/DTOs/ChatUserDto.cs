namespace Application.Features.Chats.DTOs;

/// <summary>
/// 聊天用户
/// </summary>
[Map(typeof(User))]
public class ChatUserDto
{
    public long? Id { get; set; }
    public long? UserId { get { return Id; } }
    public string? UserName { get; set; }
    public string? Realname { get; set; }
    public string? Nickname { get; set; }
    public string? ProfilePictureDataUrl { get; set; }
    public bool? IsLive { get; set; }
}
