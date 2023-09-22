using System.ComponentModel.DataAnnotations;
using Application.Features.Users.Caching;
using Domain.Entities;
using Masuit.Tools;
using Masuit.Tools.Systems;
using Microsoft.Extensions.Options;

namespace Application.Features.Users.Commands.Add;

/// <summary>
/// 添加用户
/// </summary>
[Map(typeof(User))]
public class AddUserCommand : ICacheInvalidatorRequest<Result<long>>
{
    /// <summary>
    /// 用户名
    /// </summary>
    [Required(ErrorMessage = "用户名是必填的")]
    public string UserName { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    [Required(ErrorMessage = "密码是必填的")]
    public string Password { get; set; }

    /// <summary>
    /// 确认密码
    /// </summary>
    [Required(ErrorMessage = "确认密码是必填的")]
    public string ConfirmPassword { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 手机号码
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// 用户状态：是否激活
    /// </summary>
    [Required(ErrorMessage = "用户状态是必填的")]
    public bool IsActive { get; set; }

    /// <summary>
    /// 角色唯一标识
    /// </summary>
    public List<long>? RoleIds { get; set; }

    /// <summary>
    /// 上级节点
    /// </summary>
    public long? SuperiorId { get; set; }

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
public class AddUserCommandHandler : IRequestHandler<AddUserCommand, Result<long>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IOptions<SystemSettings> _optSystemSettings;

    public AddUserCommandHandler(
        IApplicationDbContext context,
        IMapper mapper,
        IOptions<SystemSettings> optSystemSettings)
    {
        _context = context;
        _mapper = mapper;
        _optSystemSettings = optSystemSettings;
    }

    /// <summary>
    /// 业务逻辑
    /// </summary>
    /// <param name="request">请求参数</param>
    /// <param name="cancellationToken">取消标记</param>
    /// <returns>返回处理结果</returns>
    public async Task<Result<long>> Handle(AddUserCommand request, CancellationToken cancellationToken)
    {
        var user = _mapper.Map<User>(request);
        user.PasswordHash = user.CreatePassword(request.Password);
        user.ProfilePictureDataUrl = $"{_optSystemSettings.Value.HostDomainName}/Files/Image/2.png";
        user.AddDomainEvent(new CreatedEvent<User>(user));
        request?.RoleIds?.Distinct()?.ForEach(roleId =>
        {
            user.UserRoles.Add(new UserRole
            {
                Id = SnowFlake.GetInstance().GetLongId(),
                RoleId = roleId
            });
        });
        await _context.Users.AddAsync(user);
        var isSuccess = await _context.SaveChangesAsync(cancellationToken) > 0;
        return await Result<long>.SuccessOrFailureAsync(user.Id,isSuccess,new string[] { "操作失败"});
    }
}
