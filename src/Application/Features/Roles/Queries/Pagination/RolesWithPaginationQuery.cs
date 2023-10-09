using Application.Common.Extensions;
using Application.Features.Roles.Caching;
using Application.Features.Roles.DTOs;
using Application.Features.Users.Specifications;
using Ardalis.Specification;
using Domain.Entities;

namespace Application.Features.Roles.Queries.Pagination;

[Description("分页查询角色数据")]
public class RolesWithPaginationQuery : RoleAdvancedFilter, ICacheableRequest<Result<PaginatedData<RoleDto>>>
{
    public override string ToString()
    {
        return $"Search:{Keyword},RoleName:{RoleName},RoleCode:{RoleCode},IsActive:{IsActive} {OrderBy}, {SortDirection}, {PageNumber}, {PageSize}";

    }

    [JsonIgnore]
    public string CacheKey => RoleCacheKey.GetPaginationCacheKey($"{this}");
    [JsonIgnore]
    public MemoryCacheEntryOptions? Options => RoleCacheKey.MemoryCacheEntryOptions;
    [JsonIgnore]
    public RoleAdvancedPaginationSpec Specification => new RoleAdvancedPaginationSpec(this);
}

public class RolesWithPaginationQueryHandler :
         IRequestHandler<RolesWithPaginationQuery, Result<PaginatedData<RoleDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public RolesWithPaginationQueryHandler(
        IApplicationDbContext context,
        IMapper mapper
        )
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<PaginatedData<RoleDto>>> Handle(RolesWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var roles = await _context.Roles.OrderBy($"{request.OrderBy} {request.SortDirection}")
                .ProjectToPaginatedDataAsync<Role, RoleDto>(request.Specification, request.PageNumber, request.PageSize, _mapper.ConfigurationProvider, cancellationToken);
        return await Result<PaginatedData<RoleDto>>.SuccessAsync(roles);
    }
}