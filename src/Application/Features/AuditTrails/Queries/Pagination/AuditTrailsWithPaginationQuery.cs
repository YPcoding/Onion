using Application.Common.Extensions;
using Application.Features.AuditTrails.Specifications;
using Domain.Entities.Audit;
using Domain.Repositories;

namespace Application.Features.AuditTrails.Queries.Pagination;

public class AuditTrailsWithPaginationQuery: AuditTrailAdvancedFilter, IRequest<Result<PaginatedData<AuditTrail>>>
{
    [JsonIgnore]
    public AuditTrailAdvancedPaginationSpec Specification => new AuditTrailAdvancedPaginationSpec(this);
}

/// <summary>
/// 处理程序
/// </summary>
public class AuditTrailsWithPaginationQueryHandler :
IRequestHandler<AuditTrailsWithPaginationQuery, Result<PaginatedData<AuditTrail>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IRoleRepository _roleRepository;
    private readonly IMapper _mapper;
    public AuditTrailsWithPaginationQueryHandler(
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
    public async Task<Result<PaginatedData<AuditTrail>>> Handle(AuditTrailsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var auditTrails = await _context.AuditTrails
            .OrderBy($"{request.OrderBy} {request.SortDirection}")
            .ProjectToPaginatedDataAsync<AuditTrail, AuditTrail>(request.Specification, request.PageNumber, request.PageSize, _mapper.ConfigurationProvider, cancellationToken);

        return await Result<PaginatedData<AuditTrail>>.SuccessAsync(auditTrails);
    }
}