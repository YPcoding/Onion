using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Application.Features.Users.Caching;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;

namespace Application.Features.Users.Commands.Add;

/// <summary>
/// 添加用户
/// </summary>
[Map(typeof(User))]
[Description("新增用户")]
public class AddUserCommand : ICacheInvalidatorRequest<Result<long>>
{
    /// <summary>
    /// 用户名
    /// </summary>
    [Required(ErrorMessage = "用户名是必填的")]
    public string UserName { get; set; }

    /// <summary>
    /// 真实姓名
    /// </summary>
    [Description("真实姓名")]
    public virtual string? Realname { get; set; }

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
    /// 部门唯一标识
    /// </summary>
    public long? DepartmentId { get; set; }

    /// <summary>
    /// 头像图片
    /// </summary>
    public string? ProfilePictureDataUrl { get; set; }

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
    private readonly ISnowFlakeService _snowFlakeService;
    private readonly IMapper _mapper;
    private readonly IOptions<SystemSettings> _optSystemSettings;

    public AddUserCommandHandler(
        IApplicationDbContext context,
        IMapper mapper,
        IOptions<SystemSettings> optSystemSettings,
        ISnowFlakeService snowFlakeService)
    {
        _context = context;
        _mapper = mapper;
        _optSystemSettings = optSystemSettings;
        _snowFlakeService = snowFlakeService;
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
        user.AddDomainEvent(new CreatedEvent<User>(user));
        if (request.RoleIds != null)
        {
            foreach (var roleId in request.RoleIds.Distinct())
            {
                user.UserRoles.Add(new UserRole
                {
                    Id = _snowFlakeService.GenerateId(),
                    RoleId = roleId
                });
            }
        }
        await _context.Users.AddAsync(user);
        var isSuccess = await _context.SaveChangesAsync(cancellationToken) > 0;
        return await Result<long>.SuccessOrFailureAsync(user.Id,isSuccess,new string[] { "操作失败"});
    }
}
