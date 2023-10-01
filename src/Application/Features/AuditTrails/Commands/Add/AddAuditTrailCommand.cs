using System.ComponentModel.DataAnnotations;
using Domain.Entities.Audit;
using Application.Features.AuditTrails.Caching;
using Domain.Entities;
using Microsoft.Extensions.Options;

namespace Application.Features.AuditTrails.Commands.Add;

/// <summary>
/// 添加审计日志
/// </summary>
[Map(typeof(AuditTrail))]
public class AddAuditTrailCommand : IRequest<Result<long>>
{
        
        /// <summary>
        /// 关联用户
        /// </summary>
        [Description("关联用户")]
        public User Owner { get; set; }
        
        /// <summary>
        /// 审计类型
        /// </summary>
        [Description("审计类型")]
        public AuditType AuditType { get; set; }
        
        /// <summary>
        /// 表名
        /// </summary>
        [Description("表名")]
        public string TableName { get; set; }
        
        /// <summary>
        /// 审计时间
        /// </summary>
        [Description("审计时间")]
        public DateTime DateTime { get; set; }
        
        /// <summary>
        /// 旧值
        /// </summary>
        [Description("旧值")]
        public Dictionary<string, object> OldValues { get; set; }
        
        /// <summary>
        /// 新值
        /// </summary>
        [Description("新值")]
        public Dictionary<string, object> NewValues { get; set; }
        
        /// <summary>
        /// 受影响的列
        /// </summary>
        [Description("受影响的列")]
        public List<string> AffectedColumns { get; set; }
        
        /// <summary>
        /// 主关键字
        /// </summary>
        [Description("主关键字")]
        public Dictionary<string, object> PrimaryKey { get; set; }
        
        /// <summary>
        /// 临时属性
        /// </summary>
        [Description("临时属性")]
        public List<PropertyEntry> TemporaryProperties { get; set; }
        
        /// <summary>
        /// 具有临时属性
        /// </summary>
        [Description("具有临时属性")]
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
        //audittrail.AddDomainEvent(new CreatedEvent<AuditTrail>(audittrail));
        await _context.AuditTrails.AddAsync(audittrail);
        var isSuccess = await _context.SaveChangesAsync(cancellationToken) > 0;
        return await Result<long>.SuccessOrFailureAsync(audittrail.Id, isSuccess, new string[] { "操作失败" });
    }
}