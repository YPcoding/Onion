using Application.Common.Extensions;
using Application.Common.Models;
using Application.Features.Users.Caching;
using Application.Features.Users.DTOs;
using Application.Features.Users.Specifications;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json.Serialization;

namespace Application.Features.Users.Queries.Pagination;

/// <summary>
/// 用户分页查询
/// </summary>
public class UsersWithPaginationQuery : UserAdvancedFilter, ICacheableRequest<Result<PaginatedData<UserDto>>>
{
    public override string ToString()
    {
        return
            $"Search:{Keyword},UserName:{UserName},Email:{Email},EmailConfirmed:{EmailConfirmed},PhoneNumber:{PhoneNumber},LockoutEnabled:{LockoutEnabled},SortDirection:{SortDirection},OrderBy:{OrderBy},{PageNumber},{PageSize}";
    }
    [JsonIgnore]
    public UserAdvancedSpecification Specification => new UserAdvancedSpecification(this);
    public string CacheKey => UserCacheKey.GetPaginationCacheKey($"{this}");

    [JsonIgnore]
    public MemoryCacheEntryOptions? Options => UserCacheKey.MemoryCacheEntryOptions;
}
public class UsersWithPaginationQueryHandler :
IRequestHandler<UsersWithPaginationQuery, Result<PaginatedData<UserDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    public UsersWithPaginationQueryHandler(
    IApplicationDbContext context,
    IMapper mapper
    )
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<PaginatedData<UserDto>>> Handle(UsersWithPaginationQuery request,
        CancellationToken cancellationToken)
    {
        var data = await _context.Users.OrderBy($"{request.OrderBy} {request.SortDirection}")
                        .ProjectToPaginatedDataAsync<User, UserDto>(request.Specification, request.PageNumber, request.PageSize, _mapper.ConfigurationProvider, cancellationToken);
        return await Result<PaginatedData<UserDto>>.SuccessAsync(data);
    }
}
