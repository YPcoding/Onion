using Application.Common.Extensions;
using Domain.Entities.Loggers;
using Application.Features.Loggers.Caching;
using Application.Features.Loggers.DTOs;
using Application.Features.Loggers.Specifications;

namespace Application.Features.Loggers.Queries.Pagination;

/// <summary>
/// 日志分页查询
/// </summary>
public class LoggersWithPaginationQuery : LoggerAdvancedFilter, IRequest<Result<PaginatedData<LoggerDto>>>
{
    [JsonIgnore]
    public LoggerAdvancedPaginationSpec Specification => new LoggerAdvancedPaginationSpec(this);
}

/// <summary>
/// 处理程序
/// </summary>
public class LoggersWithPaginationQueryHandler :
    IRequestHandler<LoggersWithPaginationQuery, Result<PaginatedData<LoggerDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    public LoggersWithPaginationQueryHandler(
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
    /// <returns>返回日志分页数据</returns>
    public async Task<Result<PaginatedData<LoggerDto>>> Handle(
        LoggersWithPaginationQuery request,
        CancellationToken cancellationToken)
    {
        var loggers = await _context.Loggers
            .OrderBy($"{request.OrderBy} {request.SortDirection}")
            .ProjectToPaginatedDataAsync<Logger, LoggerDto>(
            request.Specification,
            request.PageNumber,
            request.PageSize,
            _mapper.ConfigurationProvider,
            cancellationToken);

        return await Result<PaginatedData<LoggerDto>>.SuccessAsync(loggers);
    }
}
