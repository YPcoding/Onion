using Infrastructure.Constants.Database;

namespace Infrastructure.Extensions;

internal static class DbContextOptionsBuilderExtensions
{
    internal static DbContextOptionsBuilder UseDatabase(this DbContextOptionsBuilder builder, string dbProvider,
        string connectionString)
    {
        switch (dbProvider.ToLowerInvariant())
        {
            case DbProviderKeys.Npgsql:
                AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
                return builder.UseNpgsql(connectionString,
                    e => e.MigrationsAssembly("Migrators.PostgreSQL"));
            case DbProviderKeys.SqlServer:
                return builder.UseSqlServer(connectionString,
                    e => e.MigrationsAssembly("Migrators.MSSQL"));
            case DbProviderKeys.SqLite:
                return builder.UseSqlite(connectionString,
                    e => e.MigrationsAssembly("Migrators.SqLite"));
            case DbProviderKeys.MySql:
                return builder.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 25)),
                    e => e.MigrationsAssembly("Migrators.MySql"));
            default:
                throw new InvalidOperationException($"DB Provider {dbProvider} is not supported.");
        }
    }
}
