using Application.Common.Helper;
using Domain.ValueObjects;
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

    public IEnumerable<Menu> GeneratePermission(IEnumerable<Menu> menus) 
    {
        var apis = WebApiDocHelper.GetWebApiControllersWithActions();
        var newMenus = new List<Menu>();
        string pattern = @"/\{[^}]+\}";
        foreach (var menu in menus)
        {
            if (menu.Meta.Title =="计划任务") 
            {
                string a = "";
            }
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
