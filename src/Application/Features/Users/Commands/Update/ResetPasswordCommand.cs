using Application.Features.Users.Caching;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.Users.Commands.Update;

/// <summary>
/// 重置密码
/// </summary>
[Map(typeof(User))]
public class ResetPasswordCommand : ICacheInvalidatorRequest<Result<long>>
{
    /// <summary>
    /// 唯一标识
    /// </summary>
    [Required(ErrorMessage = "唯一标识必填的")]
    public long UserId { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    [Required(ErrorMessage = "密码是必填的")]
    public string Password { get; set; }

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
public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result<long>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public ResetPasswordCommandHandler(
        IApplicationDbContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// 业务逻辑
    /// </summary>
    /// <param name="request">请求参数</param>
    /// <param name="cancellationToken">取消标记</param>
    /// <returns>返回处理结果</returns>
    /// <exception cref="NotFoundException">未找到的异常处理</exception>
    public async Task<Result<long>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.SingleOrDefaultAsync(x => x.Id == request.UserId
            && x.ConcurrencyStamp == request.ConcurrencyStamp, cancellationToken)
            ?? throw new NotFoundException($"数据【{request.UserId}-{request.ConcurrencyStamp}】未找到");

        user = _mapper.Map(request, user);
        user.ChangePassword(request.Password);
        user.AddDomainEvent(new UpdatedEvent<User>(user));
        _context.Users.Update(user);
        var isSuccess = await _context.SaveChangesAsync(cancellationToken) > 0;
        return await Result<long>.SuccessOrFailureAsync(
            request!.UserId,
            isSuccess,
            new string[] { "操作失败" });
    }
}
