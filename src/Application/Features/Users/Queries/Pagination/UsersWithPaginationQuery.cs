using Application.Common.Extensions;
using Application.Features.Users.Caching;
using Application.Features.Users.DTOs;
using Application.Features.Users.Specifications;
using Domain.Repositories;

namespace Application.Features.Users.Queries.Pagination;

/// <summary>
/// 用户分页查询
/// </summary>
[Description("分页查询用户数据")]
public class UsersWithPaginationQuery : UserAdvancedFilter, ICacheableRequest<Result<PaginatedData<UserDto>>>
{
    public override string ToString()
    {
        return
            $"Search:{Keyword},UserName:{UserName},Email:{Email},EmailConfirmed:{EmailConfirmed},PhoneNumber:{PhoneNumber},LockoutEnabled:{LockoutEnabled},DepartmentId:{DepartmentId},SortDirection:{SortDirection},OrderBy:{OrderBy},{PageNumber},{PageSize}";
    }
    [JsonIgnore]
    public UserAdvancedPaginationSpec Specification => new UserAdvancedPaginationSpec(this);
    [JsonIgnore]
    public string CacheKey => UserCacheKey.GetPaginationCacheKey($"{this}");
    [JsonIgnore]
    public MemoryCacheEntryOptions? Options => UserCacheKey.MemoryCacheEntryOptions;
}

/// <summary>
/// 处理程序
/// </summary>
public class UsersWithPaginationQueryHandler :
IRequestHandler<UsersWithPaginationQuery, Result<PaginatedData<UserDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IRoleRepository _roleRepository;
    private readonly IMapper _mapper;
    public UsersWithPaginationQueryHandler(
    IApplicationDbContext context,
    IMapper mapper,
    IRoleRepository roleRepository)
    {
        _context = context;
        _mapper = mapper;
        _roleRepository = roleRepository;
    }

    /// <summary>
    /// 业务逻辑
    /// </summary>
    /// <param name="request">请求参数</param>
    /// <param name="cancellationToken">取消标记</param>
    /// <returns>返回用户分页数据</returns>
    public async Task<Result<PaginatedData<UserDto>>> Handle(
        UsersWithPaginationQuery request, 
        CancellationToken cancellationToken)
    {
        var users = await _context.Users
            .OrderBy($"{request.OrderBy} {request.SortDirection}")
            .ProjectToPaginatedDataAsync<User, UserDto>(
            request.Specification, 
            request.PageNumber, 
            request.PageSize, 
            _mapper.ConfigurationProvider, 
            cancellationToken);

        var userRoles = await _roleRepository
            .GetUserRolesAsync(r => r.UserRoles.Any(ur => users.Items.Select(s => s.UserId).Contains(ur.UserId)));

        if (userRoles!.Any())
        {
            foreach (var user in users.Items) 
            {
                var roles = userRoles!.Where(x => x.UserRoles.Any(u => u.UserId == user.Id));
                if (roles.Any()) 
                {
                    user.Roles = _mapper.Map(roles, user.Roles);
                }
            }
        }

        return await Result<PaginatedData<UserDto>>.SuccessAsync(users);
    }
}
