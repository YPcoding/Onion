using Application.Features.Permissions.Caching;
using Application.Features.Permissions.DTOs;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
using Domain.Services;
using Masuit.Tools;

namespace Application.Features.Permissions.Queries.GetByUserId;

/// <summary>
/// 获取登录者权限路由
/// </summary>
public class GetLoginerPermissionRouterQuery : ICacheableRequest<Result<List<PermissionRouterDto>>>
{
    /// <summary>
    /// 用户唯一标识
    /// </summary>
    public required long UserId { get; set; }
    [JsonIgnore]
    public string CacheKey => PermissionCacheKey.GetPermissionByUserIdCacheKey(UserId);
    [JsonIgnore]
    public MemoryCacheEntryOptions? Options => PermissionCacheKey.MemoryCacheEntryOptions;
}

public class GetLoginerPermissionRouterQueryHandler :
     IRequestHandler<GetLoginerPermissionRouterQuery, Result<List<PermissionRouterDto>>>
{
    private readonly PermissionDomainService _permissionService;
    private readonly IPermissionRepository _permissionRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetLoginerPermissionRouterQueryHandler(
        IApplicationDbContext context,
        IMapper mapper,
        PermissionDomainService permissionService,
        IRoleRepository roleRepository,
        IPermissionRepository permissionRepository)
    {
        _context = context;
        _mapper = mapper;
        _permissionService = permissionService;
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
    }

    public async Task<Result<List<PermissionRouterDto>>> Handle(GetLoginerPermissionRouterQuery request, CancellationToken cancellationToken)
    {
        //var permissions = new List<Permission>()
        //{
        //    new Permission(){Id = 1, Code ="permission", Path = "/permission", Description = "menus.permission",Icon = "lollipop",Sort=10, Type=PermissionType.Menu },
        //    new Permission(){Id = 2, Code ="/permission/page/index", SuperiorId = 1, Path = "/permission/page/index", Label = "PermissionPage", Type=PermissionType.Page,Description="menus.permissionPage"},
        //    new Permission(){Id = 3, Code ="/permission/button/index", SuperiorId = 1, Path = "/permission/button/index", Label = "PermissionButton", Type=PermissionType.Page,Description="menus.permissionPage"}

        //};

        //// var userPermissions = await _permissionRepository.GetPermissionsByUserIdAsync(request.UserId);
        //var userPermissions = permissions;
        //if (!userPermissions.Any()) 
        //    return await Result<List<PermissionRouterDto>>.SuccessAsync(new List<PermissionRouterDto>());

        //var datas1 = new List<PermissionRouterDto>();
        //var menus = permissions.Where(x => x.Type == PermissionType.Menu).ToList();
        //foreach (var menu in menus)
        //{
        //    var a = new PermissionRouterDto
        //    {
        //        Path = menu.Path!,
        //        Meta = new Meta
        //        {
        //            Title = menu.Description!,
        //            Icon = menu.Icon,
        //            Rank = menu.Sort,
        //            Roles = (await _roleRepository.GetAllAsync(r => r.RolePermissions.Any(rp => rp.PermissionId == menu.Id)))?
        //                                                  .Select(s => s.RoleName.ToLower()).ToArray(),
        //        }
        //    };
        //    a.Children = Array.Empty<PermissionRouterDto>();
        //    var childrens = userPermissions.Where(x => x.SuperiorId == menu.Id && x.Type == PermissionType.Page).ToList() ?? new List<Permission>();
        //    if (childrens.Any()) 
        //    {
        //        foreach (var children in childrens)
        //        {
        //            var pagePermissionDots = permissions.Where(x => x.SuperiorId == children.Id && x.Type == PermissionType.Dot).ToList();
        //            var page = new PermissionRouterDto
        //            {
        //                Path = children.Path!,
        //                Name = children.Label,
        //                Meta = new Meta
        //                {
        //                    Title = children.Description!,
        //                    Roles = (await _roleRepository.GetAllAsync(r => r.RolePermissions.Any(rp => rp.PermissionId == children.Id)))?
        //                                                  .Select(s => s.RoleName.ToLower()).ToArray(),
        //                    Auths = pagePermissionDots?.Select(s => s.Code)?.ToArray()!,
        //                }
        //            };

        //            a.Children!.Append(page);
        //        }
        //    }

        //    datas1.Add(a);
        //}

        var datas = new List<PermissionRouterDto>();
        var permissionData = new PermissionRouterDto   //菜单
        {
            Path = "/permission",
            Meta = new Meta
            {
                Title = "menus.permission",
                Icon = "lollipop",
                Rank = 10,
            },
            Children = new PermissionRouterDto[]  //页面
            {
                new PermissionRouterDto
                {
                    Path = "/permission/page/index",
                    Name="PermissionPage",
                    Meta = new Meta
                    {
                        Title = "menus.permissionPage",
                        Roles = new[] { "admin", "common" } //角色
                    }
                },
                new PermissionRouterDto
                {
                    Path = "/permission/button/index",
                    Name="PermissionButton",
                    Meta = new Meta
                    {
                        Title = "menus.permissionPage",
                        Roles = new[] { "admin", "common" },
                        Auths= new[] { "btn_add", "btn_edit","btn_delete" }, //权限点
                    }
                },
                new PermissionRouterDto
                {
                    Path = "/permission/user/index",
                    Name="UserPage",
                    Meta = new Meta
                    {
                        Title = "用户管理",
                        Roles = new[] { "admin", "common" },
                        Auths= new[] { "btn_add", "btn_edit","btn_delete" }, //权限点
                    }
                }
             }
        };

        var systemData = new PermissionRouterDto   //菜单
        {
            Path = "/system",
            Meta = new Meta
            {
                Title = "系统管理",
                Icon = "lollipop",
                Rank = 11,
            },
            Children = new PermissionRouterDto[]  //页面
            {
                new PermissionRouterDto
                {
                    Path = "/system/user/index",
                    Name="SystemUserPage",
                    Meta = new Meta
                    {
                        Title = "用户管理",
                        Roles = new[] { "admin", "common" } //角色
                    }
                },
                new PermissionRouterDto
                {
                    Path = "/system/role/index",
                    Name="SystemRolePage",
                    Meta = new Meta
                    {
                        Title = "角色管理",
                        Roles = new[] { "admin", "common" } //角色
                    }
                }
             }
        };
        datas.Add(permissionData);
        datas.Add(systemData);
        return await Result<List<PermissionRouterDto>>.SuccessAsync(datas);
    }
}