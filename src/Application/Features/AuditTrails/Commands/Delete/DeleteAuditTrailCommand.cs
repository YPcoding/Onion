using Application.Features.AuditTrails.Caching;
using Domain.Entities.Audit;
using Domain.Entities;

namespace Application.Features.AuditTrails.Commands.Delete;

/// <summary>
/// 删除审计日志
/// </summary>
public class DeleteAuditTrailCommand : IRequest<Result<bool>>
{
  
        /// <summary>
        /// 唯一标识
        /// </summary>
        [Description("唯一标识")]
        public List<long> AuditTrailIds { get; set; }
}

/// <summary>
/// 处理程序
/// </summary>
public class DeleteAuditTrailCommandHandler : IRequestHandler<DeleteAuditTrailCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public DeleteAuditTrailCommandHandler(
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
    /// <returns>返回处理结果</returns>
    public async Task<Result<bool>> Handle(DeleteAuditTrailCommand request, CancellationToken cancellationToken)
    {
        var audittrailsToDelete = await _context.AuditTrails
            .Where(x => request.AuditTrailIds.Contains(x.Id))
            .ToListAsync();

        if (audittrailsToDelete.Any())
        {
            _context.AuditTrails.RemoveRange(audittrailsToDelete);
            var isSuccess = await _context.SaveChangesAsync(cancellationToken) > 0;
            return await Result<bool>.SuccessOrFailureAsync(isSuccess, isSuccess,new string[] {"操作失败"});
        }

        return await Result<bool>.FailureAsync(new string[] { "没有找到需要删除的数据" });
    }
}