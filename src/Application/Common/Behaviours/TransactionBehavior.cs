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
            string transactionId = Guid.NewGuid().ToString();
            _logger.LogTrace("数据库事务ID：{TransactionId},操作类型：{Type}", transactionId,"开始事务");
            using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var response = await next();
                await _dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                _logger.LogTrace("数据库事务ID：{TransactionId},操作类型：{Type}", transactionId, "提交事务"); 
                return response;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogTrace("数据库事务ID：{TransactionId},操作类型：{Type}", transactionId, "回滚事务");
                throw;
            }
        }
        else
        {
            return await next();
        }
    }
}





