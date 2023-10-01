using Application.Features.Loggers.Caching;
using Domain.Entities.Logger;
using Domain.Entities;

namespace Application.Features.Loggers.Commands.Delete;

/// <summary>
/// 删除日志
/// </summary>
public class DeleteLoggerCommand : IRequest<Result<bool>>
{
  
        /// <summary>
        /// 唯一标识
        /// </summary>
        [Description("唯一标识")]
        public List<long> LoggerIds { get; set; }
}

/// <summary>
/// 处理程序
/// </summary>
public class DeleteLoggerCommandHandler : IRequestHandler<DeleteLoggerCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public DeleteLoggerCommandHandler(
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
    public async Task<Result<bool>> Handle(DeleteLoggerCommand request, CancellationToken cancellationToken)
    {
        var loggersToDelete = await _context.Loggers
            .Where(x => request.LoggerIds.Contains(x.Id))
            .ToListAsync();

        if (loggersToDelete.Any())
        {
            _context.Loggers.RemoveRange(loggersToDelete);
            var isSuccess = await _context.SaveChangesAsync(cancellationToken) > 0;
            return await Result<bool>.SuccessOrFailureAsync(isSuccess, isSuccess,new string[] {"操作失败"});
        }

        return await Result<bool>.FailureAsync(new string[] { "没有找到需要删除的数据" });
    }
}