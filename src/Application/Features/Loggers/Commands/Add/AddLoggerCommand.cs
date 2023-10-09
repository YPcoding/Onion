using System.ComponentModel.DataAnnotations;
using Domain.Entities.Loggers;
using Application.Features.Loggers.Caching;
using Domain.Entities;
using Microsoft.Extensions.Options;

namespace Application.Features.Loggers.Commands.Add;

/// <summary>
/// 添加日志
/// </summary>
[Map(typeof(Logger))]
[Description("添加日志")]
public class AddLoggerCommand : IRequest<Result<long>>
{
        
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
public class AddLoggerCommandHandler : IRequestHandler<AddLoggerCommand, Result<long>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public AddLoggerCommandHandler(
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
    public async Task<Result<long>> Handle(AddLoggerCommand request, CancellationToken cancellationToken)
    {
        var logger = _mapper.Map<Logger>(request);
        //logger.AddDomainEvent(new CreatedEvent<Logger>(logger));
        await _context.Loggers.AddAsync(logger);
        var isSuccess = await _context.SaveChangesAsync(cancellationToken) > 0;
        return await Result<long>.SuccessOrFailureAsync(logger.Id, isSuccess, new string[] { "操作失败" });
    }
}