using Application.Common.Extensions;
using Domain.Entities.Audit;
using Application.Features.AuditTrails.Caching;
using Application.Features.AuditTrails.DTOs;
using Application.Features.AuditTrails.Specifications;

namespace Application.Features.AuditTrails.Queries.Pagination;

/// <summary>
/// 审计日志分页查询
/// </summary>
public class AuditTrailsWithPaginationQuery : AuditTrailAdvancedFilter, IRequest<Result<PaginatedData<AuditTrailDto>>>
{
    [JsonIgnore]
    public AuditTrailAdvancedPaginationSpec Specification => new AuditTrailAdvancedPaginationSpec(this);
}

/// <summary>
/// 处理程序
/// </summary>
public class AuditTrailsWithPaginationQueryHandler :
    IRequestHandler<AuditTrailsWithPaginationQuery, Result<PaginatedData<AuditTrailDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    public AuditTrailsWithPaginationQueryHandler(
        IApplicationDbContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// 业务逻辑
    /// </summary>
    /// <param name="request">请求参数</param>
    /// <param name="cancellationToken">取消标记</param>
    /// <returns>返回审计日志分页数据</returns>
    public async Task<Result<PaginatedData<AuditTrailDto>>> Handle(
        AuditTrailsWithPaginationQuery request,
        CancellationToken cancellationToken)
    {
        var audittrails = await _context.AuditTrails
            .OrderBy($"{request.OrderBy} {request.SortDirection}")
            .ProjectToPaginatedDataAsync<AuditTrail, AuditTrailDto>(
            request.Specification,
            request.PageNumber,
            request.PageSize,
            _mapper.ConfigurationProvider,
            cancellationToken);

        return await Result<PaginatedData<AuditTrailDto>>.SuccessAsync(audittrails);
    }
}
