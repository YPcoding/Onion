using Application.Common.Extensions;
using Domain.Entities.Audit;
using Application.Features.AuditTrails.Caching;
using Application.Features.AuditTrails.DTOs;
using Application.Features.AuditTrails.Specifications;
using AutoMapper.QueryableExtensions;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.AuditTrails.Queries.GetById;

/// <summary>
/// 通过唯一标识获取一条数据
/// </summary>
public class GetAuditTrailQueryById : IRequest<Result<AuditTrailDto>>
{
    /// <summary>
    /// 唯一标识
    /// </summary>
    [Required(ErrorMessage = "唯一标识必填的")]
    public long AuditTrailId { get; set; }
}

/// <summary>
/// 处理程序
/// </summary>
public class GetAuditTrailByIdQueryHandler :IRequestHandler<GetAuditTrailQueryById, Result<AuditTrailDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAuditTrailByIdQueryHandler(
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
    public async Task<Result<AuditTrailDto>> Handle(GetAuditTrailQueryById request, CancellationToken cancellationToken)
    {
        var audittrail = await _context.AuditTrails.ApplySpecification(new AuditTrailByIdSpec(request.AuditTrailId))
                     .ProjectTo<AuditTrailDto>(_mapper.ConfigurationProvider)
                     .SingleOrDefaultAsync(cancellationToken) ?? throw new NotFoundException($"唯一标识: [{request.AuditTrailId}] 未找到");
        return await Result<AuditTrailDto>.SuccessAsync(audittrail);
    }
}
