using Application.Common.Configurations;
using Domain.Entities;
using Domain.Entities.Identity;
using Domain.Enums;
using Infrastructure.Persistence;
using Masuit.Tools;
using Masuit.Tools.Systems;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Linq.Dynamic.Core;
using static Application.Common.Helper.WebApiDocHelper;

namespace CInfrastructure.Persistence;
public class ApplicationDbContextInitializer
{
    private readonly ILogger<ApplicationDbContextInitializer> _logger;
    private readonly ApplicationDbContext _context;
    private readonly IOptions<SystemSettings> _optSystemSettings;

    public ApplicationDbContextInitializer(
        ILogger<ApplicationDbContextInitializer> logger,
        ApplicationDbContext context,
        IOptions<SystemSettings> optSystemSettings)
    {
        _logger = logger;
        _context = context;
        _optSystemSettings = optSystemSettings;
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
        var menuId = SnowFlake.GetInstance().GetLongId();
        var menu = new Permission("系统管理", "系统管理", "/system", 0, "", PermissionType.Menu, "", "lollipop")
        {
            Id = menuId
        };
        foreach (var controller in controllers)
        {
            int sort = 0;
            var pageId = SnowFlake.GetInstance().GetLongId();
            var pagePath = "";
            var name = "";
            if (controller.ControllerName == "User") 
            {
                pagePath = "/system/user/index";
                name = "SystemUserPage";
            }
            if (controller.ControllerName == "Role")
            {
                pagePath = "/system/role/index";
                name = "SystemRolePage";
            }
            if (controller.ControllerName == "Permission")
            {
                pagePath = "/system/permission/index";
                name = "SystemPermissionPage";
            }
            var page = new Permission(controller.ControllerDescription, controller.ControllerDescription, pagePath, 0, "", PermissionType.Page, name, "")
            {
                Id = pageId,
                SuperiorId = menuId
            };
            allPermissions.Add(page);
            foreach (var action in controller.Actions)
            {
                var path = $"api/{controller.ControllerName}/{action.Route}";
                if (action.Description.IsNullOrEmpty()) continue;
                var permission = new Permission(controller.ControllerDescription, action.Description, path, sort++, action.HttpMethods, PermissionType.Dot, "", "")
                {
                    Id = SnowFlake.GetInstance().GetLongId(),
                    SuperiorId = pageId
                };
                allPermissions.Add(permission);
            }
            sort = 0;
        }
        allPermissions.Add(menu);
        return allPermissions;
    }

    private async Task TrySeedAsync()
    {
        // 默认用户
        var administrator = new User { UserName = "admin", IsActive = true, Email = "761516331@qq.com", EmailConfirmed = true, ProfilePictureDataUrl = $"{_optSystemSettings.Value.HostDomainName}/Files/Image/2.png" };
        administrator.PasswordHash = administrator.CreatePassword("admin");
        var user = new User { LockoutEnabled = true, UserName = "user", IsActive = true, Email = "1103354424@qq.com", EmailConfirmed = true, ProfilePictureDataUrl = $"{_optSystemSettings.Value.HostDomainName}/Files/Image/1.jpg" };
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

        // 默认用户角色
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
                _context.Permissions.AddRange(permissions);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"成功添加权限数据");
            }
        }

        // 默认角色权限
        permissions = await _context.Permissions.ToListAsync();
        var administratorRolePermissions = await _context.RolePermissions.Where(x => x.RoleId == administratorRoleId).ToListAsync();
        var userRolePermissions = await _context.RolePermissions.Where(x => x.RoleId == userRoleId).ToListAsync();
        var notHaveAdministratorPermissions = permissions.Where(x => !administratorRolePermissions.Select(x => x.PermissionId).Contains(x.Id)).ToList();
        var notHaveUserPermissions = permissions.Where(x => !userRolePermissions.Select(x => x.PermissionId).Contains(x.Id) && x.HttpMethods == "GET").ToList();
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

        //创建前端模拟权限数据
        //if (!permissions.Any(x=>x.Path== "/permission"))
        //{
        //    var permissionMenu = new Permission()
        //    {
        //        Code = "permission",
        //        Path = "/permission",
        //        Description = "menus.permission",
        //        Icon = "lollipop",
        //        Sort = 10,
        //        Type = PermissionType.Menu
        //    };
        //    await _context.Permissions.AddAsync(permissionMenu);
        //    await _context.SaveChangesAsync();
        //    permissions = new List<Permission>()
        //    {
        //        new Permission(){ Code ="/permission/page/index", SuperiorId = permissionMenu.Id, Path = "/permission/page/index", Label = "PermissionPage", Type=PermissionType.Page,Description="menus.permissionPage"},
        //        new Permission(){ Code ="/permission/button/index", SuperiorId = permissionMenu.Id, Path = "/permission/button/index", Label = "PermissionButton", Type=PermissionType.Page,Description="menus.permissionPage"}
        //    };
        //    await _context.Permissions.AddRangeAsync(permissions);
        //    await _context.SaveChangesAsync();
        //    _logger.LogInformation($"成功创建前端模拟权限数据");
        //}
    }
}
