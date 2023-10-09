using Application.Features.Roles.Caching;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.Roles.Commands.Update;


/// <summary>
/// 更改是否激活
/// </summary>
[Map(typeof(Role))]
[Description("激活或禁用角色")]
public class UpdatedRoleIsActiveCommand : ICacheInvalidatorRequest<Result<bool>>
{
    /// <summary>
    /// 角色唯一标识
    /// </summary>
    [Required(ErrorMessage = "角色唯一标识必填")]
    public long RoleId { get; set; }

    /// <summary>
    /// 是否激活
    /// </summary>
    [Required(ErrorMessage = "是否激活必填")]
    public bool IsActive { get; set; }

    /// <summary>
    /// 并发标记
    /// </summary>
    [Required(ErrorMessage = "并发标记必填的")]
    public string ConcurrencyStamp { get; set; }
    [JsonIgnore]
    public string CacheKey => RoleCacheKey.GetAllCacheKey;
    [JsonIgnore]
    public CancellationTokenSource? SharedExpiryTokenSource => RoleCacheKey.SharedExpiryTokenSource(); 
}

/// <summary>
/// 处理程序
/// </summary>
public class UpdatedRoleIsActiveCommandHandler : IRequestHandler<UpdatedRoleIsActiveCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UpdatedRoleIsActiveCommandHandler(
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
    public async Task<Result<bool>> Handle(UpdatedRoleIsActiveCommand request, CancellationToken cancellationToken)
    {
        var role = await _context.Roles
           .SingleOrDefaultAsync(x => x.Id == request.RoleId && x.ConcurrencyStamp == request.ConcurrencyStamp, cancellationToken)
           ?? throw new NotFoundException($"数据【{request.RoleId}-{request.ConcurrencyStamp}】未找到");

        role = _mapper.Map(request, role);
        role.AddDomainEvent(new UpdatedEvent<Role>(role));
        _context.Roles.Update(role);
        var isSuccess = await _context.SaveChangesAsync(cancellationToken) > 0;
        return await Result<bool>.SuccessOrFailureAsync(isSuccess, isSuccess, new string[] { "操作失败" });
    }
}