using Application.Common.Extensions;
using Domain.Entities;
using Application.Features.TestTables.Caching;
using Application.Features.TestTables.DTOs;
using Application.Features.TestTables.Specifications;

namespace Application.Features.TestTables.Queries.Pagination;

/// <summary>
/// 测试表分页查询
/// </summary>
public class TestTablesWithPaginationQuery : TestTableAdvancedFilter, IRequest<Result<PaginatedData<TestTableDto>>>
{
    [JsonIgnore]
    public TestTableAdvancedPaginationSpec Specification => new TestTableAdvancedPaginationSpec(this);
}

/// <summary>
/// 处理程序
/// </summary>
public class TestTablesWithPaginationQueryHandler :
    IRequestHandler<TestTablesWithPaginationQuery, Result<PaginatedData<TestTableDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    public TestTablesWithPaginationQueryHandler(
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
    /// <returns>返回测试表分页数据</returns>
    public async Task<Result<PaginatedData<TestTableDto>>> Handle(
        TestTablesWithPaginationQuery request,
        CancellationToken cancellationToken)
    {
        var testtables = await _context.TestTables
            .OrderBy($"{request.OrderBy} {request.SortDirection}")
            .ProjectToPaginatedDataAsync<TestTable, TestTableDto>(
            request.Specification,
            request.PageNumber,
            request.PageSize,
            _mapper.ConfigurationProvider,
            cancellationToken);

        return await Result<PaginatedData<TestTableDto>>.SuccessAsync(testtables);
    }
}
