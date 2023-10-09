using Application.Features.Users.Caching;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.Users.Commands.Update;

/// <summary>
/// 修改个人信息
/// </summary>
[Map(typeof(User))]
[Description("修改个人信息")]
public class UpdateUserInfoCommand : ICacheInvalidatorRequest<Result<long>>
{
    /// <summary>
    /// 并发标记
    /// </summary>
    [Required(ErrorMessage = "并发标记必填的")]
    public string ConcurrencyStamp { get; set; }

    /// <summary>
    /// 真实姓名
    /// </summary>
    [Description("真实姓名")]
    public virtual string? Realname { get; set; }

    /// <summary>
    /// 昵称
    /// </summary>
    [Description("昵称")]
    public virtual string? Nickname { get; set; }

    /// <summary>
    /// 性别
    /// </summary>
    [Description("性别")]
    public virtual GenderType GenderType { get; set; }

    /// <summary>
    /// 生日
    /// </summary>
    [Description("生日")]
    public DateTime? Birthday { get; set; }

    /// <summary>
    /// 个性签名
    /// </summary>
    [Description("个性签名")]
    public string? Signature { get; set; }

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
public class UpdateUserInfoCommandHandler : IRequestHandler<UpdateUserInfoCommand, Result<long>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public UpdateUserInfoCommandHandler(
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
    public async Task<Result<long>> Handle(UpdateUserInfoCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.SingleOrDefaultAsync(x => x.Id == _currentUserService.CurrentUserId
            && x.ConcurrencyStamp == request.ConcurrencyStamp, cancellationToken)
            ?? throw new NotFoundException($"数据【{_currentUserService.CurrentUserId}-{request.ConcurrencyStamp}】未找到");

        user = _mapper.Map(request, user);
        user.AddDomainEvent(new UpdatedEvent<User>(user));
        _context.Users.Update(user);
        var isSuccess = await _context.SaveChangesAsync(cancellationToken) > 0;
        return await Result<long>.SuccessOrFailureAsync(_currentUserService.CurrentUserId, isSuccess, new string[] { "操作失败" });
    }
}
