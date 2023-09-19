using Domain.Entities;
using Domain.Entities.Identity;
using Infrastructure.Persistence;
using Masuit.Tools;
using Masuit.Tools.Systems;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using static Application.Common.Helper.WebApiDocHelper;

namespace CInfrastructure.Persistence;
public class ApplicationDbContextInitializer
{
    private readonly ILogger<ApplicationDbContextInitializer> _logger;
    private readonly ApplicationDbContext _context;

    public ApplicationDbContextInitializer(
        ILogger<ApplicationDbContextInitializer> logger,
        ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }
    public async Task InitialiseAsync()
    {
        try
        {
            if (_context.Database.IsSqlServer() || _context.Database.IsNpgsql() || _context.Database.IsSqlite())
            {
                await _context.Database.MigrateAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "初始化数据库时出错");
            throw;
        }
    }
    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
            _context.ChangeTracker.Clear();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "为数据库设定种子时出错");
            throw;
        }
    }
    private static List<Permission> GetAllPermissions()
    {
        List<ControllerInfo> controllers = GetWebApiControllersWithActions();
        var allPermissions = new List<Permission>();
        foreach (var controller in controllers)
        {
            int sort = 0;
            foreach (var action in controller.Actions)
            {
                var path = $"api/{controller.ControllerName}/{action.Route}";
                var permission = new Permission(controller.ControllerDescription, action.Description, path, sort++, action.HttpMethods)
                {
                    Id = SnowFlake.GetInstance().GetLongId()
                };
                allPermissions.Add(permission);
            }
            sort = 0;
        }
        return allPermissions;
    }

    private async Task TrySeedAsync()
    {
        // 默认用户
        var administrator = new User { UserName = "admin", IsActive = true, Email = "761516331@qq.com", EmailConfirmed = true, ProfilePictureDataUrl = "https://p.qqan.com/up/2021-4/16182800486083341.png" };   
        administrator.PasswordHash= administrator.CreatePassword("admin");
        var user = new User { UserName = "user", IsActive = true, Email = "1103354424@qq.com", EmailConfirmed = true, ProfilePictureDataUrl = "https://p.qqan.com/up/2021-3/16157755391319120.jpg" };
        user.PasswordHash = user.CreatePassword("user");
        if (!await _context.Users.AnyAsync(x => x.UserName == "admin"))
        {
            _context.Users.Add(administrator);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"成功添加管理员数据");
        }
        if (!await _context.Users.AnyAsync(x => x.UserName == "user"))
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"成功添加普通用户数据");
        }

        // 默认角色
        var administratorRole = new Role("Admin", "系统管理员");
        var userRole = new Role("User", "普通用户");
        if (!await _context.Roles.AnyAsync(x => x.RoleName == "Admin"))
        {
            _context.Roles.Add(administratorRole);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"成功添加管理员角色数据");
        }
        if (!await _context.Roles.AnyAsync(x => x.RoleName == "User"))
        {
            _context.Roles.Add(userRole);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"成功添加普通用户角色数据");
        }

        // 默认用户角色
        var administratorRoleId = (await _context.Roles.FirstOrDefaultAsync(x => x.RoleName == "Admin"))?.Id;
        var administratorId = (await _context.Users.FirstOrDefaultAsync(x => x.UserName == "admin"))?.Id;
        var userRoleId = (await _context.Roles.FirstOrDefaultAsync(x => x.RoleName == "User"))?.Id;
        var userId = (await _context.Users.FirstOrDefaultAsync(x => x.UserName == "user"))?.Id;
        if (!await _context.UserRoles.AnyAsync(x => x.UserId == administratorId && x.RoleId == administratorRoleId))
        {
            var administratorUserRole = new UserRole { UserId = (long)administratorId!, RoleId = (long)administratorRoleId! };
            _context.UserRoles.Add(administratorUserRole);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"成功添加管理员角色数据");
        }
        if (!await _context.UserRoles.AnyAsync(x => x.UserId == userId && x.RoleId == userRoleId))
        {
            var userUserRole = new UserRole { UserId = (long)userId!, RoleId = (long)userRoleId! };
            _context.UserRoles.Add(userUserRole);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"成功添加普通用户角色数据");
        }

        // 默认权限
        var permissions = GetAllPermissions();
        if (permissions.Any())
        {
            var hasPermissions = await _context.Permissions.ToListAsync();
            if (hasPermissions.Any())
            {
                permissions = permissions.Where(x => !hasPermissions.Select(s => s.Code).Contains(x.Code)).ToList();
            }
            if (permissions.Any())
            {
                var administratorPermissions = permissions;
                var userPermissions = permissions.Where(x => x.HttpMethods?.ToUpper() == "GET");
                _context.Permissions.AddRange(administratorPermissions);
                if (userPermissions.Any())
                {
                    _context.Permissions.AddRange(userPermissions);
                }
                await _context.SaveChangesAsync();
                _logger.LogInformation($"成功添加权限数据");
            }
        }

        // 默认角色权限
        permissions = await _context.Permissions.ToListAsync();
        var administratorRolePermissions = await _context.RolePermissions.Where(x => x.RoleId == administratorRoleId).ToListAsync();
        var userRolePermissions = await _context.RolePermissions.Where(x => x.RoleId == userRoleId).ToListAsync();
        var notHaveAdministratorPermissions = permissions.Where(x => !administratorRolePermissions.Select(x => x.PermissionId).Contains(x.Id)).ToList();
        var notHaveUserPermissions = permissions.Where(x => !userRolePermissions.Select(x => x.PermissionId).Contains(x.Id)).ToList();
        if (notHaveAdministratorPermissions.Any())
        {
            var items = new List<RolePermission>();
            foreach (var permission in notHaveAdministratorPermissions)
            {
                var item = new RolePermission
                {
                    Id = SnowFlake.GetInstance().GetLongId(),
                    PermissionId = permission.Id,
                    RoleId = (long)administratorRoleId!
                };
                items.Add(item);
            }
            _context.AddRange(items);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"成功添加管理员权限数据");
        }
        if (notHaveUserPermissions.Any())
        {
            var items = new List<RolePermission>();
            foreach (var permission in notHaveUserPermissions) 
            {
                var item = new RolePermission
                {
                    Id = SnowFlake.GetInstance().GetLongId(),
                    PermissionId = permission.Id,
                    RoleId = (long)userRoleId!
                };
                items.Add(item);
            }
            _context.AddRange(items);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"成功添加普通用户权限数据");
        }
    }
}
