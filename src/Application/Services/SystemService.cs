using Application.Common.Helper;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.AccessControl;
using System.Text.RegularExpressions;

namespace Application.Services;

public class SystemService : IScopedDependency
{
    private readonly ISnowFlakeService _snowFlakeService;
    public SystemService(ISnowFlakeService snowFlakeService)
    {
        _snowFlakeService = snowFlakeService;
    }
    public class SystemMenuSetting
    {
        public string Grop { get; set; }
        public string Label { get; set; }
        public string Path { get; set; }
        public string Icon { get; set; }
    }

    public IEnumerable<Permission> GenerateMenus(IEnumerable<Permission>? excludeMenus)
    {
        var apis = WebApiDocHelper.GetWebApiControllersWithActions();
        var grops = apis.GroupBy(g => g.Group);
        var allPermissions = new List<Permission>();

        IEnumerable<SystemMenuSetting> settings = new List<SystemMenuSetting>
        {
            new SystemMenuSetting { Grop="系统管理", Label="系统管理", Path="/system", Icon="ep:setting" },
            new SystemMenuSetting { Grop="日志管理", Label="日志管理", Path="/lg", Icon="ep:coin" },
            new SystemMenuSetting { Grop="代码生成器", Label="代码生成器", Path="/genrate", Icon="ep:opportunity" }
        };

        var hiddenPages = new List<string>()
        {
            "授权管理",
            "文件上传管理",
            "测试接口"
        };
        var menuOrder = 0;
        foreach (var menu in grops)
        {
            var setting = settings.FirstOrDefault(x => x.Grop == menu.Key)!;
            if (setting == null) 
            {
                setting = new SystemMenuSetting()
                {
                    Grop = menu.Key,
                    Label = menu.Key,
                    Path = $"/{menu?.FirstOrDefault()?.ControllerName?.ToLower()}",
                    Icon = "lollipop",
                };
            }
            var menuId = _snowFlakeService.GenerateId();
            allPermissions.Add(new Permission($"{setting.Grop}", $"{setting.Label}", $"{setting.Path}", menuOrder++, "", PermissionType.Menu, "", $"{setting.Icon}")
            {
                Id = menuId
            });

            var pageOrder = 0;
            foreach (var page in menu!)
            {
                var pageId = _snowFlakeService.GenerateId();
                var pagePath = $"{setting.Path}/{page.ControllerName.ToLower()}/index";
                var name = $"{setting.Path.Replace("/", "").ToTitleCase()}{page.ControllerName}Page";
                allPermissions.Add(new Permission(page.ControllerDescription, page.ControllerDescription, pagePath, pageOrder++, "", PermissionType.Page, name, "")
                {
                    Id = pageId,
                    SuperiorId = menuId
                });

                var dotOrder = 0;
                foreach (var dot in page.Actions)
                {
                    var path = $"api/{page.ControllerName}/{dot.Route}";
                    if (dot.Description.IsNullOrEmpty()) continue;
                    allPermissions.Add(new Permission(page.ControllerDescription, dot.Description, path, dotOrder++, dot.HttpMethods, PermissionType.Dot, "", "")
                    {
                        Id = _snowFlakeService.GenerateId(),
                        SuperiorId = pageId
                    });
                }
                dotOrder = 0;
            }
            pageOrder = 0;
        }

        if (excludeMenus != null && excludeMenus.Any())
        {
            allPermissions = allPermissions.Where(x => !excludeMenus.Select(s => s.Code).Contains(x.Code)).ToList();
        }

        if (allPermissions != null && excludeMenus != null
           && allPermissions.Any() && excludeMenus.Any())
        {
            //避免重复添加
            allPermissions.ForEach(permission =>
            {
                if (permission.Type == PermissionType.Dot)
                {
                    var permissionDotToReplaceSuperior = excludeMenus
                      .Where(x => x.Group == permission.Group && x.Type == PermissionType.Dot)
                      .FirstOrDefault();
                    if (permissionDotToReplaceSuperior != null)
                    {
                        permission.SuperiorId = permissionDotToReplaceSuperior.SuperiorId;
                    }
                }
                if (permission.Type == PermissionType.Page)
                {
                    var permissionPageToReplaceSuperior = excludeMenus
                      .Where(x => permission.Code!.StartsWith(x.Code!) && x.Superior == null && x.Type == PermissionType.Menu)
                      .FirstOrDefault();
                    if (permissionPageToReplaceSuperior != null)
                    {
                        permission.SuperiorId = permissionPageToReplaceSuperior.Id;
                    }
                }
            });
        }

        if (hiddenPages.Any()) 
        {
            allPermissions?.ForEach(p => 
            {
                if (hiddenPages.Contains(p.Label!))
                {
                    p.Hidden = true;
                }
            });
        }

        return allPermissions ?? new List<Permission>();
    }

    public IEnumerable<Menu> GeneratePermission(IEnumerable<Menu> menus) 
    {
        var apis = WebApiDocHelper.GetWebApiControllersWithActions();
        var newMenus = new List<Menu>();
        string pattern = @"/\{[^}]+\}";
        foreach (var menu in menus)
        {
            var api = apis.FirstOrDefault(x => x.ControllerDescription == menu.Meta.Title);
            if (api == null) continue;

            foreach (var button in api.Actions)
            {
                if (menus.Any(x=>x.Meta.Type==MetaType.Button && button.Description == x.Meta.Title))
                    continue;

                // 创建按钮
                var btn = new Menu
                {
                    Id = _snowFlakeService.GenerateId(),
                    ParentId = menu.Id,
                    Name = $"{api.ControllerName.ToLower()}{button.Route}",
                    Meta = new Meta(MetaType.Button, button.Description, null, null, null, null, null, null, null)
                };
                btn.Name = Regex.Replace(btn.Name, pattern, string.Empty).Replace("/","");
                newMenus.Add(btn);

                // 创建接口权限
                var permission = new Menu
                {
                    Id = _snowFlakeService.GenerateId(),
                    ParentId = btn.Id,
                    Code = $"{api.ControllerName}.{button.Route}".ToLower(),
                    Url = $"/api/{api.ControllerName}/{button.Route}",
                    Meta = new Meta(MetaType.Api, null, null, null, null, null, null, null, null)
                };
                permission.Code = Regex.Replace(permission.Code, pattern, string.Empty).Replace("/", "");
                permission.Url = Regex.Replace(permission.Url, pattern, string.Empty);
                newMenus.Add(permission);
            }
        }

        return newMenus;
    }
}
