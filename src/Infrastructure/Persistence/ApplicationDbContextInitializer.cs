using Application.Common.Configurations;
using Application.Services;
using Domain.Entities;
using Domain.Entities.Identity;
using Domain.Enums;
using Infrastructure.Persistence;

using Microsoft.Extensions.Options;
using System.Linq.Dynamic.Core;

namespace CInfrastructure.Persistence;
public class ApplicationDbContextInitializer
{
    private readonly ILogger<ApplicationDbContextInitializer> _logger;
    private readonly ApplicationDbContext _context;
    private readonly IOptions<SystemSettings> _optSystemSettings;
    private readonly SystemService _systemService;
    private readonly ISnowFlakeService _snowFlakeService;

    public ApplicationDbContextInitializer(
        ILogger<ApplicationDbContextInitializer> logger,
        ApplicationDbContext context,
        IOptions<SystemSettings> optSystemSettings,
        SystemService systemService,
        ISnowFlakeService snowFlakeService)
    {
        _logger = logger;
        _context = context;
        _optSystemSettings = optSystemSettings;
        _systemService = systemService;
        _snowFlakeService = snowFlakeService;
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

    private async Task TrySeedAsync()
    {
        // 生成默认用户
        var administrator = new User { UserName = "admin", IsActive = true, Email = "761516331@qq.com", EmailConfirmed = true, ProfilePictureDataUrl = $"{_optSystemSettings.Value.HostDomainName}/Image/2.png" };
        administrator.PasswordHash = administrator.CreatePassword("admin");
        var user = new User { LockoutEnabled = true, UserName = "user", IsActive = true, Email = "1103354424@qq.com", EmailConfirmed = true, ProfilePictureDataUrl = $"{_optSystemSettings.Value.HostDomainName}/Image/1.jpg" };
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

        // 生成默认角色
        var administratorRole = new Role() { RoleName = "超级管理员", RoleCode = "Administrator", Description = "拥有系统所有权限", IsActive = true };
        var userRole = new Role() { RoleName = "普通用户", RoleCode = "Common", Description = "拥有系统所有查询功能", IsActive = true };
        if (!await _context.Roles.AnyAsync(x => x.RoleCode == "Administrator"))
        {
            _context.Roles.Add(administratorRole);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"成功添加管理员角色数据");
        }
        if (!await _context.Roles.AnyAsync(x => x.RoleCode == "Common"))
        {
            _context.Roles.Add(userRole);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"成功添加普通用户角色数据");
        }

        // 为用户添加角色
        var administratorRoleId = (await _context.Roles.FirstOrDefaultAsync(x => x.RoleCode == "Administrator"))?.Id;
        var administratorId = (await _context.Users.FirstOrDefaultAsync(x => x.UserName == "admin"))?.Id;
        var userRoleId = (await _context.Roles.FirstOrDefaultAsync(x => x.RoleCode == "Common"))?.Id;
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

        // 生成权限
        var excludePermissions = await _context.Permissions.ToListAsync();
        var generatePermissions = _systemService.GenerateMenus(excludePermissions);
        if (generatePermissions.Any())
        {
            _context.Permissions.AddRange(generatePermissions);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"成功添加权限数据");
        }

        // 添加角色权限
        var permissions = await _context.Permissions.ToListAsync();
        var administratorRolePermissions = await _context.RolePermissions.Where(x => x.RoleId == administratorRoleId).ToListAsync();
        var userRolePermissions = await _context.RolePermissions.Where(x => x.RoleId == userRoleId).ToListAsync();
        var notHaveAdministratorPermissions = permissions.Where(x => !administratorRolePermissions.Select(x => x.PermissionId).Contains(x.Id)).ToList();
        var notHaveUserPermissions = permissions.Where(x => !userRolePermissions.Select(x => x.PermissionId).Contains(x.Id) && (x.HttpMethods == "GET" || x.Type == PermissionType.Page || x.Type == PermissionType.Menu)).ToList();
        if (notHaveAdministratorPermissions.Any())
        {
            var items = new List<RolePermission>();
            foreach (var permission in notHaveAdministratorPermissions)
            {
                var item = new RolePermission
                {
                    Id = _snowFlakeService.GenerateId(),
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
                    Id = _snowFlakeService.GenerateId(),
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
