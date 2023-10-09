using Application.Features.Roles.Caching;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.Roles.Commands.Add;


/// <summary>
/// 添加角色
/// </summary>
[Map(typeof(Role))]
[Description("新增角色")]
public class AddRoleCommand : ICacheInvalidatorRequest<Result<long>>
{
    /// <summary>
    /// 角色名称
    /// </summary>
    [Required(ErrorMessage = "角色名称必填")]
    public string RoleName { get; set; }

    /// <summary>
    /// 角色标识
    /// </summary>
    [Required(ErrorMessage = "角色标识必填")]
    public string RoleCode { get; set; }

    /// <summary>
    /// 是否激活
    /// </summary>
    [Required(ErrorMessage = "是否激活必填")]
    public bool IsActive { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [Required(ErrorMessage = "排序必填")]
    public int Sort { get; set; }

    /// <summary>
    /// 角色描述
    /// </summary>
    [Required(ErrorMessage = "角色描述必填")]
    public string Description { get; set; }

    [JsonIgnore]
    public string CacheKey => RoleCacheKey.GetAllCacheKey;
    [JsonIgnore]
    public CancellationTokenSource? SharedExpiryTokenSource => RoleCacheKey.SharedExpiryTokenSource(); 
}

/// <summary>
/// 处理程序
/// </summary>
public class AddRoleCommandHandler : IRequestHandler<AddRoleCommand, Result<long>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public AddRoleCommandHandler(
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
    public async Task<Result<long>> Handle(AddRoleCommand request, CancellationToken cancellationToken)
    {
        var role = _mapper.Map<Role>(request);
        role.AddDomainEvent(new CreatedEvent<Role>(role));
        await _context.Roles.AddAsync(role);
        var isSuccess = await _context.SaveChangesAsync(cancellationToken) > 0;
        return await Result<long>.SuccessOrFailureAsync(role.Id, isSuccess, new string[] { "操作失败" });
    }
}