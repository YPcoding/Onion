namespace Infrastructure.Persistence;

public class ContextFactory<TContext> : IDbContextFactory<TContext> where TContext : DbContext
{
    private readonly IServiceProvider _provider;

    public ContextFactory(IServiceProvider provider)
    {
        this._provider = provider;
    }

    public TContext CreateDbContext()
    {
        if (_provider == null)
        {
            throw new InvalidOperationException(
                $"You must configure an instance of IServiceProvider");
        }

        return ActivatorUtilities.CreateInstance<TContext>(_provider);
    }
}
