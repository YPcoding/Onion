using Application.Features.Users.Caching;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.Users.Commands.Update;

/// <summary>
/// 修改用户
/// </summary>
[Map(typeof(User))]
[Description("修改用户信息")]
public class UpdateUserCommand : ICacheInvalidatorRequest<Result<long>>
{
    /// <summary>
    /// 唯一标识
    /// </summary>
    [Required(ErrorMessage = "唯一标识必填的")]
    public long UserId { get; set; }

    /// <summary>
    /// 真实姓名
    /// </summary>
    [Description("真实姓名")]
    public virtual string? Realname { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 是否激活
    /// </summary>
    [Required(ErrorMessage = "是否激活是必填的")]
    public bool IsActive { get; set; }

    /// <summary>
    /// 手机号码
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// 角色唯一标识
    /// </summary>
    public List<long>? RoleIds { get; set; }

    /// <summary>
    /// 上级节点
    /// </summary>
    public long? SuperiorId { get; set; }

    /// <summary>
    /// 部门唯一标识
    /// </summary>
    public long? DepartmentId { get; set; }

    /// <summary>
    /// 头像图片
    /// </summary>
    public string? ProfilePictureDataUrl { get; set; }

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
public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result<long>>
{
    private readonly IApplicationDbContext _context;
    private readonly ISnowFlakeService _snowFlakeService;
    private readonly IMapper _mapper;

    public UpdateUserCommandHandler(
        IApplicationDbContext context,
        IMapper mapper,
        ISnowFlakeService snowFlakeService)
    {
        _context = context;
        _mapper = mapper;
        _snowFlakeService = snowFlakeService;
    }

    /// <summary>
    /// 业务逻辑
    /// </summary>
    /// <param name="request">请求参数</param>
    /// <param name="cancellationToken">取消标记</param>
    /// <returns>返回处理结果</returns>
    /// <exception cref="NotFoundException">未找到的异常处理</exception>
    public async Task<Result<long>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.SingleOrDefaultAsync(x => x.Id == request.UserId
            && x.ConcurrencyStamp == request.ConcurrencyStamp, cancellationToken)
            ?? throw new NotFoundException($"数据【{request.UserId}-{request.ConcurrencyStamp}】未找到");

        var userRoles = await _context.UserRoles.Where(x => x.UserId == request.UserId).ToListAsync(cancellationToken);
        if (userRoles.Any())
        {
            _context.UserRoles.RemoveRange(userRoles);
        }

        request?.RoleIds?.Distinct()?.ForEach(roleId =>
        {
            user.UserRoles.Add(new UserRole
            {
                Id = _snowFlakeService.GenerateId(),
                RoleId = roleId
            });
        });

        user = _mapper.Map(request, user);
        user.AddDomainEvent(new UpdatedEvent<User>(user));
        _context.Users.Update(user);
        var isSuccess = await _context.SaveChangesAsync(cancellationToken) > 0;
        return await Result<long>.SuccessOrFailureAsync(
            request!.UserId,
            isSuccess,
            new string[] { "操作失败" });
    }
}