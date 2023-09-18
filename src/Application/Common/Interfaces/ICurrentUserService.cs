namespace Application.Common.Interfaces;

public interface ICurrentUserService : IScopedDependency
{
    public string? UserId { get; }
    public long CurrentUserId { get; }
    public string? Email { get; }
    public string? UserName { get; }
    public string? TenantId { get; }
    public string? DisplayName { get; }
    public string? SuperiorId { get; }
    public string? SuperiorName { get; }
    public string? ProfilePictureDataUrl { get; }
    public string[]? AssignRoles { get; }
}
