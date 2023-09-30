using Application.Features.Users.Caching;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.Roles.Commands.Update;

/// <summary>
/// 分配角色
/// </summary>
public class AssigningRoleCommand : ICacheInvalidatorRequest<Result<bool>>
{
    /// <summary>
    /// 用户唯一标识
    /// </summary>
    [Required(ErrorMessage = "用户唯一标识必填")]
    public long UserId { get; set; }

    /// <summary>
    /// 角色唯一标识
    /// </summary>
    public List<long>? RoleIds { get; set; }

    [JsonIgnore]
    public string CacheKey => UserCacheKey.GetAllCacheKey;
    [JsonIgnore]
    public CancellationTokenSource? SharedExpiryTokenSource => UserCacheKey.SharedExpiryTokenSource();
}

/// <summary>
/// 处理程序
/// </summary>
public class AssigningRoleCommandHandler : IRequestHandler<AssigningRoleCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ISnowFlakeService _snowFlakeService;

    public AssigningRoleCommandHandler(
        IApplicationDbContext context, ISnowFlakeService snowFlakeService)
    {
        _context = context;
        _snowFlakeService = snowFlakeService;
    }

    /// <summary>
    /// 业务逻辑
    /// </summary>
    /// <param name="request">请求参数</param>
    /// <param name="cancellationToken">取消标记</param>
    /// <returns>返回处理结果</returns>
    public async Task<Result<bool>> Handle(AssigningRoleCommand request, CancellationToken cancellationToken)
    {
        var userRoles = await _context.UserRoles.Where(x => x.UserId == request.UserId).ToListAsync();
        if (userRoles.Any())
        {
            _context.UserRoles.RemoveRange(userRoles);
        }
        request.RoleIds = request.RoleIds?.Distinct().ToList();
        foreach (var roleId in request?.RoleIds!) 
        {
            await _context.UserRoles.AddAsync(new UserRole
            {
                Id = _snowFlakeService.GenerateId(),
                UserId = request.UserId,
                RoleId = roleId
            });
        }
        var isSuccess = await _context.SaveChangesAsync(cancellationToken) > 0;

        return await Result<bool>.SuccessOrFailureAsync(
            isSuccess, isSuccess, new string[] { "操作失败" });
    }
}