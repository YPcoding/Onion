using Application.Common.Interfaces;
using Ardalis.Specification.EntityFrameworkCore;
using Domain.Entities;
using Domain.Enums;
using Masuit.Tools;
using Masuit.Tools.Systems;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniExcelLibs;
using System;
using static Application.Common.Helper.WebApiDocHelper;

namespace WebAPI.Controllers;

/// <summary>
/// 测试接口
/// </summary>
public class WeatherForecastController : ApiControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IApplicationDbContext _context;

    public WeatherForecastController(
        ILogger<WeatherForecastController> logger,
        IApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    /// <summary>
    /// 接口调试
    /// </summary>
    /// <returns></returns>
    [HttpGet(Name = "GetWeatherForecast")]
    [AllowAnonymous]
    public async Task Get()
    {
        //List<ControllerInfo> controllers = GetWebApiControllersWithActions();
        //var permissions = await _context.Permissions.ToListAsync();
        //var menuGroups = controllers.GroupBy(x => x.Group).ToList();

        //var permissionsToAdd = new List<Permission>();

        //foreach (var permissionMenus in menuGroups)
        //{
        //    var permissionMenu = permissions
        //        .Where(x => x.Group == permissionMenus.Key && x.SuperiorId == null)
        //        .FirstOrDefault();
        //    if (permissionMenu == null)
        //    {
        //        permissionMenu = new Permission();
        //        if (permissionMenus.Key == "系统管理")
        //        {
        //            permissionMenu.Path = "/System";
        //            permissionMenu.Id = SnowFlake.GetInstance().GetLongId();
        //            var menu = new Permission("系统管理", "系统管理", $"{permissionMenu.Path.ToLower()}", 0, "", PermissionType.Menu, "", "lollipop")
        //            {
        //                Id = permissionMenu.Id
        //            };
        //            permissionsToAdd.Add(menu);
        //        }
        //        else
        //        {
        //            int count = permissions.GroupBy(x => x.Group).Count();
        //            string path = permissionMenus?.FirstOrDefault()!.ControllerName.ToLower()!;
        //            var menuId = SnowFlake.GetInstance().GetLongId();
        //            var menu = new Permission(permissionMenus!.Key, permissionMenus.Key, $"/{path}", count++, "", PermissionType.Menu, "", "lollipop")
        //            {
        //                Id = menuId
        //            };
        //            permissionsToAdd.Add(menu);
        //        }
        //    }
        //    foreach (var permissionPage in permissionMenus)
        //    {
        //        var pageId = SnowFlake.GetInstance().GetLongId();
        //        var pagePath = $"{permissionMenu.Path?.ToLower()}/{permissionPage.ControllerName}/index";
        //        var pageName = $"{permissionMenu.Path?.Replace("/", "")}{permissionPage.ControllerName}Page";
        //        var page = new Permission(permissionPage.ControllerDescription, permissionPage.ControllerDescription, pagePath, 0, "", PermissionType.Page, pageName, "")
        //        {
        //            Id = pageId,
        //            SuperiorId = permissionMenu.Id
        //        };
        //        var existPage = permissions
        //        .Where(x => x.Code == page.Code && x.Type == PermissionType.Page)
        //        .FirstOrDefault();
        //        if (existPage == null)
        //        {
        //            existPage = new Permission();
        //            existPage = page;
        //            permissionsToAdd.Add(page);
        //        }

        //        foreach (var permissionDot in permissionPage.Actions)
        //        {
        //            var path = $"api/{permissionPage.ControllerName}/{permissionDot.Route}";
        //            if (permissionDot.Description.IsNullOrEmpty()) continue;
        //            var dot = new Permission(permissionPage.ControllerDescription, permissionDot.Description, path, 0, permissionDot.HttpMethods, PermissionType.Dot, "", "")
        //            {
        //                Id = SnowFlake.GetInstance().GetLongId(),
        //                SuperiorId = existPage.Id
        //            };
        //            var existDot = permissions
        //                .Where(x => x.Code == page.Code && x.Type == PermissionType.Dot)
        //                .FirstOrDefault();
        //            if (existDot == null)
        //            {
        //                permissionsToAdd.Add(dot);
        //            }
        //        }
        //    }
        //}

        //if (permissionsToAdd.Any())
        //{
        //    foreach (var item in permissionsToAdd)
        //    {
        //        if (item.SuperiorId == 0)
        //        {
        //            item.SuperiorId = null;
        //        }
        //    }
        //    await _context.Permissions.AddRangeAsync(permissionsToAdd);
        //    await _context.SaveChangesAsync();
        //}
    }
}
