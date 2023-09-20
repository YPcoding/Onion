using Application.Features.Permissions.Caching;
using Application.Features.Permissions.DTOs;
using Domain.Services;

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
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetLoginerPermissionRouterQueryHandler(
        IApplicationDbContext context,
        IMapper mapper
,
        PermissionDomainService permissionService)
    {
        _context = context;
        _mapper = mapper;
        _permissionService = permissionService;
    }

    public async Task<Result<List<PermissionRouterDto>>> Handle(GetLoginerPermissionRouterQuery request, CancellationToken cancellationToken)
    {
        var datas=new List<PermissionRouterDto>();
        var data = new PermissionRouterDto
        {
            Path = "/permission",
            Meta = new Meta
            {
                Title = "menus.permission",
                Icon = "lollipop",
                Rank = 10,
            },
            Children = new PermissionRouterDto[]
             {
                new PermissionRouterDto
                {
                    Path = "/permission/page/index",
                    Name="PermissionPage",
                    Meta = new Meta
                    {
                        Title = "menus.permissionPage",
                        Roles = new[] { "admin", "common" }
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
                        Auths= new[] { "btn_add", "btn_edit","btn_delete" },
                    }
                }
             }
        };
        datas.Add(data);
        return await Result<List<PermissionRouterDto>>.SuccessAsync(datas);
    }
}
