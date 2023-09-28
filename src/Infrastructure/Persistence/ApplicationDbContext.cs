using Domain.Entities;
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
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<TestTable> TestTables { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        builder.ApplyGlobalFilters<ISoftDelete>(s => s.Deleted == null);

        //foreach (var entityType in builder.Model.GetEntityTypes())
        //{
        //    foreach (var foreignKey in entityType.GetForeignKeys())
        //    {
        //        // 删除外键约束
        //        foreignKey.DeleteBehavior = DeleteBehavior.NoAction;
        //    }
        //}
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {

        }
    }
}
