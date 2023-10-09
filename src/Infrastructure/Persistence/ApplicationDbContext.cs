using Domain.Entities;
using Domain.Entities.Departments;
using Domain.Entities.Identity;
using Domain.Entities.Logger;
using Domain.Entities.Notifications;
using Domain.Entities.Settings;
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
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<UserProfileSetting> UserProfileSettings { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Menu> Menus { get; set; }
    public DbSet<RoleMenu> RoleMenus { get; set; }

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
