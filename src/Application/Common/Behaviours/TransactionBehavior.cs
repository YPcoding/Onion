namespace Application.Common.Behaviours;

public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;

    public TransactionBehavior(IApplicationDbContext dbContext, ILogger<TransactionBehavior<TRequest, TResponse>> logger)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        //sqlite不支持这样的事务
        if (_dbContext.Database.ProviderName!.ToLower().EndsWith("sqlite") == true)
        {
            return await next();
        } 

        bool hasActiveTransaction = _dbContext.Database.CurrentTransaction != null;

        if (!hasActiveTransaction)
        {
            string transactionId = Guid.NewGuid().ToString(); // 生成一个唯一的事务ID
            _logger.LogInformation($"事务 {transactionId} 开始。"); // 记录事务开始
            using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var response = await next();
                await _dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                _logger.LogInformation($"事务 {transactionId} 提交。"); // 记录事务提交
                return response;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError($"事务 {transactionId} 回滚。"); // 记录事务回滚
                throw;
            }
        }
        else
        {
            return await next();
        }
    }
}





