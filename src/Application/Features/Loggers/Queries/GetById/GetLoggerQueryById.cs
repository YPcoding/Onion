using Application.Common.Extensions;
using Domain.Entities.Loggers;
using Application.Features.Loggers.Caching;
using Application.Features.Loggers.DTOs;
using Application.Features.Loggers.Specifications;
using AutoMapper.QueryableExtensions;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.Loggers.Queries.GetById;

/// <summary>
/// 通过唯一标识获取一条数据
/// </summary>
[Description("查询单条日志数据")]
public class GetLoggerQueryById : IRequest<Result<LoggerDto>>
{
    /// <summary>
    /// 唯一标识
    /// </summary>
    [Required(ErrorMessage = "唯一标识必填的")]
    public long LoggerId { get; set; }
}

/// <summary>
/// 处理程序
/// </summary>
public class GetLoggerByIdQueryHandler :IRequestHandler<GetLoggerQueryById, Result<LoggerDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetLoggerByIdQueryHandler(
        IApplicationDbContext context,
        IMapper mapper
        )
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// 业务逻辑
    /// </summary>
    /// <param name="request">请求参数</param>
    /// <param name="cancellationToken">取消标记</param>
    /// <returns>返回查询的一条数据</returns>
    /// <exception cref="NotFoundException">未找到数据移除处理</exception>
    public async Task<Result<LoggerDto>> Handle(GetLoggerQueryById request, CancellationToken cancellationToken)
    {
        var logger = await _context.Loggers.ApplySpecification(new LoggerByIdSpec(request.LoggerId))
                     .ProjectTo<LoggerDto>(_mapper.ConfigurationProvider)
                     .SingleOrDefaultAsync(cancellationToken) ?? throw new NotFoundException($"唯一标识: [{request.LoggerId}] 未找到");
        return await Result<LoggerDto>.SuccessAsync(logger);
    }
}
