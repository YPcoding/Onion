﻿using Application.Features.Permissions.DTOs;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;


namespace Application.Features.Permissions.Queries.GetByUserId;

/// <summary>
/// 获取登录者权限路由
/// </summary>
public class GetLoginerPermissionRouterQuery : IRequest<Result<IEnumerable<PermissionRouterDto>>>
{
    /// <summary>
    /// 用户唯一标识
    /// </summary>
    public required long UserId { get; set; }
    //[JsonIgnore]
    //public string CacheKey => PermissionCacheKey.GetPermissionByUserIdCacheKey(UserId);
    //[JsonIgnore]
    //public MemoryCacheEntryOptions? Options => PermissionCacheKey.MemoryCacheEntryOptions;
}

public class GetLoginerPermissionRouterQueryHandler :
     IRequestHandler<GetLoginerPermissionRouterQuery, Result<IEnumerable<PermissionRouterDto>>>
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly IRoleRepository _roleRepository;

    public GetLoginerPermissionRouterQueryHandler(
        IRoleRepository roleRepository,
        IPermissionRepository permissionRepository)
    {
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
    }

    public async Task<Result<IEnumerable<PermissionRouterDto>>> Handle(GetLoginerPermissionRouterQuery request, CancellationToken cancellationToken)
    {
        var permissions = await _permissionRepository.GetPermissionsByUserIdAsync(request.UserId);

        var routers = permissions
            .Where(x => x.Type == PermissionType.Menu)
            .Select(menu => new PermissionRouterDto
            {
                Id = menu.Id,
                Path = menu.Path!,
                Meta = new Meta
                {
                    Title = menu.Label!,
                    Icon = menu.Icon,
                    Rank = menu.Sort,
                },
                Children = permissions
                    .Where(p => p.SuperiorId == menu.Id && p.Type == PermissionType.Page && !string.IsNullOrEmpty(p.Description))
                    .Select(permission => new PermissionRouterDto
                    {
                        Id = permission.Id,
                        Path = permission.Path!,
                        Name = permission.Description,
                        Meta = new Meta
                        {
                            Title = permission.Label!,
                            Roles = GetRolesByPermissionIdAsync(permission.Id).Result,//角色
                            Auths = permissions.Where(x => x.SuperiorId == permission.Id)
                                    .Select(s => s.Code!.ToLower()).ToArray()
                        }
                    }).OrderBy(x => x.Meta.Rank).ToList()
            }).OrderBy(x => x.Meta.Rank).ToList();
        return await Result<IEnumerable<PermissionRouterDto>>.SuccessAsync(routers);
    }

    private async Task<string[]> GetRolesByPermissionIdAsync(long permissionId)
    {
        var roles = await _roleRepository.GetAllAsync(x => x.RolePermissions.Any(rp => rp.PermissionId == permissionId));
        if (roles!.Any()) return roles!.Select(s => s.RoleCode.ToLower()).ToArray();

        return Array.Empty<string>();
    }

    private async Task<string[]> GetAuthsByPermissionIdAsync(IEnumerable<Permission> permissions, long permissionId)
    {
        return await Task.Run(() =>
        {
            return permissions.Where(x => x.SuperiorId == permissionId)
            .Select(s => s.Code!.ToLower()).ToArray();
        });
    }
}