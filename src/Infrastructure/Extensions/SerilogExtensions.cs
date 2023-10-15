using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog;
using Common.Quartzs;

namespace Infrastructure.Extensions;

public static class SerilogExtensions
{
    public static void RegisterSerilog(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, configuration) =>
            configuration.ReadFrom.Configuration(context.Configuration)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Error)
                .MinimumLevel.Override("Serilog", LogEventLevel.Error)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore.AddOrUpdate", LogEventLevel.Error)
                .Enrich.FromLogContext()
                .Enrich.WithTime()
                .WriteTo.Async(wt => wt.File("./log/log-.txt", rollingInterval: RollingInterval.Day))
                .WriteTo.Async(wt =>
                    wt.Console(
                        outputTemplate:
                        "[{Timestamp:HH:mm:ss} {Level:u3} {ClientIp}] {Message:lj}{NewLine}{Exception}"))
                .WriteTo.Sink(new CustomDatabaseLogSink(builder.Services.BuildServiceProvider().GetRequiredService<IServiceProvider>(),
                                                        builder.Services.BuildServiceProvider().GetRequiredService<ICurrentUserService>()))
        );
    }

    public static LoggerConfiguration WithTime(this LoggerEnrichmentConfiguration enrichmentConfiguration)
    {
        return enrichmentConfiguration.With<TimestampEnricher>();
    }
}

internal class TimestampEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory pf)
    {
        logEvent.AddOrUpdateProperty(pf.CreateProperty("TimeStamp", logEvent.Timestamp.DateTime));
    }
}