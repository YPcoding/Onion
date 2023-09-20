using Application.Features.Users.Caching;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.Users.Commands.Delete;

/// <summary>
/// 删除用户
/// </summary>
public class DeleteUserCommand : ICacheInvalidatorRequest<Result<bool>>
{
    /// <summary>
    /// 删除用户的唯一标识
    /// </summary>
    [Required(ErrorMessage = "删除数据的唯一标识是必填的")]
    public List<long> UserIds { get; set; }

    /// <summary>
    /// 缓存Key值
    /// </summary>
    [JsonIgnore]
    public string CacheKey => UserCacheKey.GetAllCacheKey;

    [JsonIgnore]
    public CancellationTokenSource? SharedExpiryTokenSource => UserCacheKey.SharedExpiryTokenSource();

}

/// <summary>
/// 处理程序
/// </summary>
public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public DeleteUserCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 业务逻辑
    /// </summary>
    /// <param name="request">请求参数</param>
    /// <param name="cancellationToken">取消标记</param>
    /// <returns>返回处理结果</returns>
    public async Task<Result<bool>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var users = await _context.Users.Where(x=> request.UserIds.Contains(x.Id)).ToListAsync();
        _context.Users.RemoveRange(users);

        if (users?.Any() ?? false)
        {
            _context.Users.RemoveRange(users);

            var userRole = await _context.UserRoles
                .Where(x => users.Select(s => s.Id).Contains(x.UserId))
                .ToListAsync(cancellationToken);
            if (userRole.Any()) _context.UserRoles.RemoveRange(userRole);
        }

        var isSuccess = await _context.SaveChangesAsync(cancellationToken) > 0;
        return await Result<bool>.SuccessOrFailureAsync(
            isSuccess,
            isSuccess,
            new string[] { "操作失败" });
    }
}