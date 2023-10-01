using Application.Features.Loggers.Caching;
using Domain.Entities.Logger;
using Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.Loggers.Commands.Update;


/// <summary>
/// 修改日志
/// </summary>
[Map(typeof(Logger))]
public class UpdateLoggerCommand : IRequest<Result<long>>
{

        
        /// <summary>
        /// 唯一标识
        /// </summary>
        [Description("唯一标识")]
        public long LoggerId { get; set; }
        
        /// <summary>
        /// 消息
        /// </summary>
        [Description("消息")]
        public string Message { get; set; }
        
        /// <summary>
        /// 消息模板
        /// </summary>
        [Description("消息模板")]
        public string MessageTemplate { get; set; }
        
        /// <summary>
        /// 消息等级
        /// </summary>
        [Description("消息等级")]
        public string Level { get; set; }
        
        /// <summary>
        /// 发生时间
        /// </summary>
        [Description("发生时间")]
        public DateTime TimeStamp { get; set; }
        
        /// <summary>
        /// 异常
        /// </summary>
        [Description("异常")]
        public string Exception { get; set; }
        
        /// <summary>
        /// 用户名
        /// </summary>
        [Description("用户名")]
        public string UserName { get; set; }
        
        /// <summary>
        /// 客户端IP
        /// </summary>
        [Description("客户端IP")]
        public string ClientIP { get; set; }
        
        /// <summary>
        /// IP
        /// </summary>
        [Description("IP")]
        public string ClientAgent { get; set; }
        
        /// <summary>
        /// 特征
        /// </summary>
        [Description("特征")]
        public string Properties { get; set; }
        
        /// <summary>
        /// 日志事件
        /// </summary>
        [Description("日志事件")]
        public string LogEvent { get; set; }
}

/// <summary>
/// 处理程序
/// </summary>
public class UpdateLoggerCommandHandler : IRequestHandler<UpdateLoggerCommand, Result<long>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UpdateLoggerCommandHandler(
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
    public async Task<Result<long>> Handle(UpdateLoggerCommand request, CancellationToken cancellationToken)
    {
        var logger = await _context.Loggers
           .SingleOrDefaultAsync(x => x.Id == request.LoggerId, cancellationToken)
           ?? throw new NotFoundException($"数据【{request.LoggerId}】未找到");

        logger = _mapper.Map(request, logger);
        //logger.AddDomainEvent(new UpdatedEvent<Logger>(logger));
        _context.Loggers.Update(logger);
        var isSuccess = await _context.SaveChangesAsync(cancellationToken) > 0;
        return await Result<long>.SuccessOrFailureAsync(logger.Id, isSuccess, new string[] { "操作失败" });
    }
}
