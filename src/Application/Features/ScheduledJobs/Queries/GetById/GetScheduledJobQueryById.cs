using Application.Common.Extensions;
using Domain.Entities.Job;
using Application.Features.ScheduledJobs.Caching;
using Application.Features.ScheduledJobs.DTOs;
using Application.Features.ScheduledJobs.Specifications;
using AutoMapper.QueryableExtensions;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.ScheduledJobs.Queries.GetById;

/// <summary>
/// 通过唯一标识获取一条数据
/// </summary>
public class GetScheduledJobQueryById : IRequest<Result<ScheduledJobDto>>
{
    /// <summary>
    /// 唯一标识
    /// </summary>
    [Required(ErrorMessage = "唯一标识必填的")]
    public long ScheduledJobId { get; set; }
}

/// <summary>
/// 处理程序
/// </summary>
public class GetScheduledJobByIdQueryHandler :IRequestHandler<GetScheduledJobQueryById, Result<ScheduledJobDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetScheduledJobByIdQueryHandler(
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
    public async Task<Result<ScheduledJobDto>> Handle(GetScheduledJobQueryById request, CancellationToken cancellationToken)
    {
        var scheduledjob = await _context.ScheduledJobs.ApplySpecification(new ScheduledJobByIdSpec(request.ScheduledJobId))
                     .ProjectTo<ScheduledJobDto>(_mapper.ConfigurationProvider)
                     .SingleOrDefaultAsync(cancellationToken) ?? throw new NotFoundException($"唯一标识: [{request.ScheduledJobId}] 未找到");
        return await Result<ScheduledJobDto>.SuccessAsync(scheduledjob);
    }
}
