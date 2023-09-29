using System.ComponentModel.DataAnnotations;
using Domain.Entities.Audit;
using Application.Features.AuditTrails.Caching;
using Domain.Entities;
using Masuit.Tools.Systems;
using Microsoft.Extensions.Options;

namespace Application.Features.AuditTrails.Commands.Add;

/// <summary>
/// 添加
/// </summary>
[Map(typeof(AuditTrail))]
public class AddAuditTrailCommand : IRequest<Result<long>>
{
        
        /// <summary>
        /// 
        /// </summary>
        [Description("")]
        public string TableName { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [Description("")]
        public DateTime DateTime { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [Description("")]
        public bool HasTemporaryProperties { get; set; }
}
/// <summary>
/// 处理程序
/// </summary>
public class AddAuditTrailCommandHandler : IRequestHandler<AddAuditTrailCommand, Result<long>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public AddAuditTrailCommandHandler(
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
    public async Task<Result<long>> Handle(AddAuditTrailCommand request, CancellationToken cancellationToken)
    {
        var audittrail = _mapper.Map<AuditTrail>(request);
        await _context.AuditTrails.AddAsync(audittrail);
        var isSuccess = await _context.SaveChangesAsync(cancellationToken) > 0;
        return await Result<long>.SuccessOrFailureAsync(audittrail.Id, isSuccess, new string[] { "操作失败" });
    }
}