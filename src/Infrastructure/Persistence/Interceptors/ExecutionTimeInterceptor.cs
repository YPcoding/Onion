using System.Data.Common;
using System.Diagnostics;

namespace Infrastructure.Persistence.Interceptors;

public class ExecutionTimeInterceptor : DbCommandInterceptor
{
    private readonly Stopwatch _stopwatch = new Stopwatch();

    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
    DbCommand command,
    CommandEventData eventData,
    InterceptionResult<DbDataReader> result,
    CancellationToken cancellationToken = default)
    {
        _stopwatch.Restart();
        return base.ReaderExecutingAsync(command, eventData, result);
    }

    public override async ValueTask<DbDataReader> ReaderExecutedAsync(
    DbCommand command,
    CommandExecutedEventData eventData,
    DbDataReader result,
    CancellationToken cancellationToken = default)
    {
        _stopwatch.Stop();
        var executionTime = _stopwatch.ElapsedMilliseconds;
        Console.WriteLine($"SQL执行时间: {executionTime} ms");

        return await base.ReaderExecutedAsync(command, eventData, result);
    }
}
