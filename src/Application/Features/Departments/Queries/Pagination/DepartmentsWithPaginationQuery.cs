using Application.Common.Extensions;
using Domain.Entities.Departments;
using Application.Features.Departments.DTOs;
using Application.Features.Departments.Specifications;

namespace Application.Features.Departments.Queries.Pagination;

/// <summary>
/// 分页查询
/// </summary>
[Description("分页查询部门数据")]
public class DepartmentsWithPaginationQuery : DepartmentAdvancedFilter, IRequest<Result<PaginatedData<DepartmentDto>>>
{
    [JsonIgnore]
    public DepartmentAdvancedPaginationSpec Specification => new DepartmentAdvancedPaginationSpec(this);
}

/// <summary>
/// 处理程序
/// </summary>
public class DepartmentsWithPaginationQueryHandler :
    IRequestHandler<DepartmentsWithPaginationQuery, Result<PaginatedData<DepartmentDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    public DepartmentsWithPaginationQueryHandler(
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
    /// <returns>返回分页数据</returns>
    public async Task<Result<PaginatedData<DepartmentDto>>> Handle(
        DepartmentsWithPaginationQuery request,
        CancellationToken cancellationToken)
    {
        var departments = await _context.Departments
            .OrderBy($"{request.OrderBy} {request.SortDirection}")
            .ProjectToPaginatedDataAsync<Department, DepartmentDto>(
            request.Specification,
            request.PageNumber,
            request.PageSize,
            _mapper.ConfigurationProvider,
            cancellationToken);

        return await Result<PaginatedData<DepartmentDto>>.SuccessAsync(departments);
    }
}
