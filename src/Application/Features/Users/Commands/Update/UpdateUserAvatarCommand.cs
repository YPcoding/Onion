using Application.Features.Users.Caching;
using Domain.Repositories;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.Users.Commands.Update;

/// <summary>
/// 修改用户
/// </summary>
[Map(typeof(User))]
[Description("修改头像")]
public class UpdateUserAvatarCommand : ICacheInvalidatorRequest<Result<long>>
{
    /// <summary>
    /// 唯一标识
    /// </summary>
    [Required(ErrorMessage = "唯一标识必填的")]
    public long UserId { get; set; }

    /// <summary>
    /// 头像图片
    /// </summary>
    [Required(ErrorMessage = "头像链接必填")]
    public string ProfilePictureDataUrl { get; set; }

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
public class UpdateUserAvatarCommandHandler : IRequestHandler<UpdateUserAvatarCommand, Result<long>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;


    public UpdateUserAvatarCommandHandler(
        IApplicationDbContext context,
        IMapper mapper,
        IUserRepository userRepository)
    {
        _context = context;
        _mapper = mapper;
        _userRepository = userRepository;
    }

    /// <summary>
    /// 业务逻辑
    /// </summary>
    /// <param name="request">请求参数</param>
    /// <param name="cancellationToken">取消标记</param>
    /// <returns>返回处理结果</returns>
    /// <exception cref="NotFoundException">未找到的异常处理</exception>
    public async Task<Result<long>> Handle(UpdateUserAvatarCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .SingleOrDefaultAsync(x => x.Id == request.UserId && x.ConcurrencyStamp == request.ConcurrencyStamp, cancellationToken)
            ?? throw new NotFoundException($"数据【{request.UserId}-{request.ConcurrencyStamp}】未找到");
        user = _mapper.Map(request, user);
        user.AddDomainEvent(new UpdatedEvent<User>(user));
        var isSuccess = await _userRepository.UpdateUserAvatarUri(user);
        return await Result<long>.SuccessOrFailureAsync(request!.UserId, isSuccess, new string[] { "操作失败" });
    }
}