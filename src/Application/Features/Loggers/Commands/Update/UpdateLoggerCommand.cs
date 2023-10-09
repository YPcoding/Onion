using Application.Features.Loggers.Caching;
using Domain.Entities.Loggers;
using Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.Loggers.Commands.Update;


/// <summary>
/// 修改日志
/// </summary>
[Map(typeof(Logger))]
[Description("修改日志")]
public class UpdateLoggerCommand : IRequest<Result<long>>
{

        
        /// <summary>
        /// 
        /// </summary>
        [Description("")]
        public int LoggerId { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [Description("")]
        public string Timestamp { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [Description("")]
        public string Level { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [Description("")]
        public string Template { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [Description("")]
        public string Message { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [Description("")]
        public string Exception { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [Description("")]
        public string Properties { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [Description("")]
        public DateTime? TS { get; set; }
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
