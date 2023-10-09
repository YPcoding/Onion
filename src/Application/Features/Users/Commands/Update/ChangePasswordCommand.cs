using Application.Features.Users.Caching;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.Users.Commands.Update;

/// <summary>
/// 更改密码
/// </summary>
[Map(typeof(User))]
[Description("更改密码")]
public class ChangePasswordCommand : ICacheInvalidatorRequest<Result<bool>>
{
    /// <summary>
    /// 当前密码
    /// </summary>
    [Required(ErrorMessage = "当前密码是必填的")]
    public string CurrentPassword { get; set; }

    /// <summary>
    /// 新密码
    /// </summary>
    [Required(ErrorMessage = "新密码是必填的")]
    public string NewPassword { get; set; }

    /// <summary>
    /// 确认新密码
    /// </summary>
    [Required(ErrorMessage = "确认新密码是必填的")]
    public string ConfirmNewPassword { get; set; }

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
public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public ChangePasswordCommandHandler(
        IApplicationDbContext context,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// 业务逻辑
    /// </summary>
    /// <param name="request">请求参数</param>
    /// <param name="cancellationToken">取消标记</param>
    /// <returns>返回处理结果</returns>
    /// <exception cref="NotFoundException">未找到的异常处理</exception>
    public async Task<Result<bool>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .SingleOrDefaultAsync(x => x.Id == _currentUserService.CurrentUserId, cancellationToken);

        user = _mapper.Map(request, user);
        if (!user!.CompareWithOldPassword(request.CurrentPassword))
            return await Result<bool>.FailureAsync(new string[] { "当前密码错误" });

        user.ChangePassword(request.NewPassword);
        user.AddDomainEvent(new UpdatedEvent<User>(user));
        _context.Users.Update(user);

        var isSuccess = await _context.SaveChangesAsync(cancellationToken) > 0;
        return await Result<bool>.SuccessOrFailureAsync(isSuccess, isSuccess, new string[] { "操作失败" });
    }
}