using Application.Common.Extensions;
using Domain.Entities.Loggers;
using Application.Features.Loggers.DTOs;
using Application.Features.Loggers.Specifications;
using Microsoft.VisualBasic;
using Application.Constants.Loggers;

namespace Application.Features.Loggers.Queries.Pagination;

/// <summary>
/// 日志分页查询
/// </summary>
[Description("日志分页查询")]
public class LoggersWithPaginationQuery : LoggerAdvancedFilter, IRequest<Result<PaginatedData<LoggerDto>>>
{
    [JsonIgnore]
    public LoggerAdvancedPaginationSpec Specification => new LoggerAdvancedPaginationSpec(this);
}

/// <summary>
/// 系统日志分页查询
/// </summary>
[Description("系统日志分页查询")]
public class SystemLoggersWithPaginationQuery : SystemLoggerAdvancedFilter, IRequest<Result<PaginatedData<LoggerDto>>>
{
    [JsonIgnore]
    public SystemLoggerAdvancedPaginationSpec Specification => new SystemLoggerAdvancedPaginationSpec(this);
}

/// <summary>
/// 系统日志柱形图统计查询
/// </summary>
[Description("系统日志柱形图统计查询")]
public class SystemLoggersCountDailyQuery : IRequest<Result<CountDailyDto>>
{
}

/// <summary>
/// 处理程序
/// </summary>
public class LoggersWithPaginationQueryHandler :
    IRequestHandler<LoggersWithPaginationQuery, Result<PaginatedData<LoggerDto>>>,
    IRequestHandler<SystemLoggersWithPaginationQuery, Result<PaginatedData<LoggerDto>>>,
    IRequestHandler<SystemLoggersCountDailyQuery, Result<CountDailyDto>>

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

    /// <summary>
    /// 业务逻辑
    /// </summary>
    /// <param name="request">请求参数</param>
    /// <param name="cancellationToken">取消标记</param>
    /// <returns>返回系统日志分页数据</returns>
    public async Task<Result<PaginatedData<LoggerDto>>> Handle(
        SystemLoggersWithPaginationQuery request,
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

    /// <summary>
    /// 业务逻辑
    /// </summary>
    /// <param name="request">请求参数</param>
    /// <param name="cancellationToken">取消标记</param>
    /// <returns>返回系统日志分页数据</returns>
    public async Task<Result<CountDailyDto>> Handle(SystemLoggersCountDailyQuery request, CancellationToken cancellationToken)
    {
        var days = 14;
        var countDailyDto = new CountDailyDto();
        for (int i = days; i >= 0; i--)
        {
            countDailyDto.XAxis.Add(DateTime.Now.AddDays(-i).ToString("yyyy-MM-dd"));
        }

        long startDateTime = DateTime.Parse(countDailyDto.XAxis[0]).ToUnixTimestampMilliseconds();
        long endDateTime = DateTime.Parse(countDailyDto.XAxis[days]).AddDays(1).ToUnixTimestampMilliseconds();
        var loggers = await _context.Loggers
            .Where(x => x.TimestampLong >= startDateTime && x.TimestampLong < endDateTime && x.MessageTemplate == MessageTemplate.ActivityHistoryLog)
            .ToListAsync();

        if (loggers.Any())
        {
            foreach (var day in countDailyDto.XAxis)
            {
                int informationConut = loggers.Where(x => x.Level == "Information" && x.Timestamp!.Value.ToString("yyyy-MM-dd") == day).Count();
                int warningConut = loggers.Where(x => x.Level == "Warning" && x.Timestamp!.Value.ToString("yyyy-MM-dd") == day).Count();
                int erorConut = loggers.Where(x => (x.Level == "Error" || x.Level == "Critical" || x.Level == "Fatal") && x.Timestamp!.Value.ToString("yyyy-MM-dd") == day).Count();

                countDailyDto.InformationConut.Add(informationConut);
                countDailyDto.WarningConut.Add(warningConut);
                countDailyDto.ErrorConut.Add(erorConut);
            }
        }

        return await Result<CountDailyDto>.SuccessAsync(countDailyDto);
    }
}