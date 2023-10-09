using Application.Features.Users.Caching;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.Users.Commands.Update;

/// <summary>
/// 解锁或锁定用户
/// </summary>
[Map(typeof(User))]
[Description("解锁或锁定用户")]
public class IsUnLockUserCommand : ICacheInvalidatorRequest<Result<bool>>
{
    /// <summary>
    /// 唯一标识
    /// </summary>
    [Required(ErrorMessage = "唯一标识必填的")]
    public long UserId { get; set; }

    /// <summary>
    /// 并发标记
    /// </summary>
    [Required(ErrorMessage = "并发标记必填的")]
    public string ConcurrencyStamp { get; set; }

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
public class IsUnLockUserCommandHandler : IRequestHandler<IsUnLockUserCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;

    public IsUnLockUserCommandHandler(
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
    /// <exception cref="NotFoundException">未找到的异常处理</exception>
    public async Task<Result<bool>> Handle(IsUnLockUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.SingleOrDefaultAsync(x => x.Id == request.UserId && x.ConcurrencyStamp == request.ConcurrencyStamp, cancellationToken)
            ?? throw new NotFoundException($"数据【{request.UserId}-{request.ConcurrencyStamp}】未找到");

        user.IsUnLock();
        user.AddDomainEvent(new UpdatedEvent<User>(user));
        _context.Users.Update(user);
        var isSuccess = await _context.SaveChangesAsync(cancellationToken) > 0;
        return await Result<bool>.SuccessOrFailureAsync(isSuccess, isSuccess, new string[] { "操作失败" });
    }
}