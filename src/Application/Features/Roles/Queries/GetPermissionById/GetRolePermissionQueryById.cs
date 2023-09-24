using Application.Features.Roles.Caching;
using Application.Features.Roles.DTOs;
using Domain.Repositories;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.Roles.Queries.GetPermissionById;

public class GetRolePermissionQueryById : ICacheableRequest<Result<IEnumerable<RolePermissionDto>>>
{
    /// <summary>
    /// 唯一标识
    /// </summary>
    [Required(ErrorMessage = "唯一标识必填的")]
    public long RoleId { get; set; }
    [JsonIgnore]
    public string CacheKey => RoleCacheKey.GetPermissionByIdCacheKey(RoleId);
    [JsonIgnore]
    public MemoryCacheEntryOptions? Options => RoleCacheKey.MemoryCacheEntryOptions;
}

/// <summary>
/// 处理程序
/// </summary>
public class GetRolePermissionByIdQueryHandler : IRequestHandler<GetRolePermissionQueryById, Result<IEnumerable<RolePermissionDto>>>
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly IMapper _mapper;

    public GetRolePermissionByIdQueryHandler(
        IPermissionRepository permissionRepository, IMapper mapper)
    {
        _permissionRepository = permissionRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// 业务逻辑
    /// </summary>
    /// <param name="request">请求参数</param>
    /// <param name="cancellationToken">取消标记</param>
    /// <returns>返回一条查询的角色数据</returns>
    /// <exception cref="NotFoundException">未找到数据移除处理</exception>
    public async Task<Result<IEnumerable<RolePermissionDto>>> Handle(GetRolePermissionQueryById request, CancellationToken cancellationToken)
    {
        // 查询所有权限
        var allPermissions = await _permissionRepository.GetAllAsync();

        if (!allPermissions.Any())
        {
            return await Result<IEnumerable<RolePermissionDto>>.SuccessAsync(new List<RolePermissionDto>());
        }

        // 查询与指定角色相关的权限
        var rolePermissions = await _permissionRepository.GetAllAsync(x => x.RolePermissions.Any(rp => rp.RoleId == request.RoleId));

        // 映射所有权限到DTO
        var allPermissionDtos = _mapper.Map<IEnumerable<RolePermissionDto>>(allPermissions);

        // 使用HashSet来提高查找性能
        var rolePermissionIds = new HashSet<long>(rolePermissions.Select(rp => rp.Id));

        // 标记是否拥有权限
        foreach (var permissionDto in allPermissionDtos)
        {
            permissionDto.Has = rolePermissionIds.Contains(permissionDto.Id);
        }

        return await Result<IEnumerable<RolePermissionDto>>.SuccessAsync(allPermissionDtos);
    }
}
