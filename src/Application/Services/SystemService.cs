using Application.Common.Helper;

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
            new SystemMenuSetting { Grop="系统管理", Label="系统管理", Path="/system", Icon="lollipop" },
            new SystemMenuSetting { Grop="日志管理", Label="日志管理", Path="/log", Icon="lollipop" },
            new SystemMenuSetting { Grop="代码生成器", Label="代码生成器", Path="/genrate", Icon="lollipop" }
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
}
