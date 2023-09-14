namespace Application.Common.Interfaces;

public interface ICurrentUserService : IScopedDependency
{
    string? UserId { get; set; }
    string? UserName { get; set; }
}
