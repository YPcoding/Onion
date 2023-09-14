using Domain.Entities.Identity;
using Domain.Entities.Logger;
using Infrastructure.Persistence.Extensions;
using System.Reflection;

namespace Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options
    ) : base(options)
    {

    }
    public DbSet<User> Users { get ; set ; }
    public DbSet<AuditTrail> AuditTrails { get; set; }
    public DbSet<Logger> Loggers { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        builder.ApplyGlobalFilters<ISoftDelete>(s => s.Deleted == null);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {

        }
    }
}
