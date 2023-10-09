using Application.Features.Roles.Caching;
using Application.Features.Roles.DTOs;
using Domain.Repositories;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.Roles.Queries.GetMenusById;

[Description("通过唯一标识角色数据")]
public class GetRoleMenuQueryById : ICacheableRequest<Result<IEnumerable<RoleMenuDto>>>
{
    /// <summary>
    /// 唯一标识
    /// </summary>
    [Required(ErrorMessage = "唯一标识必填的")]
    public long RoleId { get; set; }
    [JsonIgnore]
    public string CacheKey => RoleCacheKey.GetMenuByIdCacheKey(RoleId);
    [JsonIgnore]
    public MemoryCacheEntryOptions? Options => RoleCacheKey.MemoryCacheEntryOptions;
}

/// <summary>
/// 处理程序
/// </summary>
public class GetRoleMenuQueryByIdHandler : IRequestHandler<GetRoleMenuQueryById, Result<IEnumerable<RoleMenuDto>>>
{
    private readonly IMenuRepository _menuRepository;
    private readonly IMapper _mapper;

    public GetRoleMenuQueryByIdHandler( IMapper mapper, IMenuRepository menuRepository)
    {
        _mapper = mapper;
        _menuRepository = menuRepository;
    }

    /// <summary>
    /// 业务逻辑
    /// </summary>
    /// <param name="request">请求参数</param>
    /// <param name="cancellationToken">取消标记</param>
    /// <returns>返回一条查询的角色数据</returns>
    /// <exception cref="NotFoundException">未找到数据移除处理</exception>
    public async Task<Result<IEnumerable<RoleMenuDto>>> Handle(GetRoleMenuQueryById request, CancellationToken cancellationToken)
    {
        // 查询所有权限
        var allmenus = await _menuRepository.GetAllAsync(x => x.Meta.Type != MetaType.Api);

        if (!allmenus.Any())
        {
            return await Result<IEnumerable<RoleMenuDto>>.SuccessAsync(new List<RoleMenuDto>());
        }

        // 查询与指定角色相关的权限
        var roleMenus = await _menuRepository.GetAllAsync(x => x.RoleMenus.Any(rp => rp.RoleId == request.RoleId));

        // 映射所有权限到DTO
        var allMenuDtos = _mapper.Map<IEnumerable<RoleMenuDto>>(allmenus);

        // 使用HashSet来提高查找性能
        var roleMenuIds = new HashSet<long>(roleMenus.Select(rp => rp.Id));

        // 标记是否拥有权限
        foreach (var menuDto in allMenuDtos)
        {
            menuDto.Has = roleMenuIds.Contains(menuDto.Id);
        }

        return await Result<IEnumerable<RoleMenuDto>>.SuccessAsync(allMenuDtos);
    }
}
