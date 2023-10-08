﻿using Application.Common.Configurations;
using Application.Services;
using Domain.Entities;
using Domain.Entities.Identity;
using Infrastructure.Persistence;

using Microsoft.Extensions.Options;
using System.Linq.Dynamic.Core;
using System.Text.Json;
using static CInfrastructure.Persistence.ApplicationDbContextInitializer;

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
            if (_context.Database.IsSqlServer() || 
                _context.Database.IsNpgsql() || 
                _context.Database.IsSqlite() || 
                _context.Database.IsMySql())
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


        var menus = await _context.Menus.ToListAsync();
        if (!menus.Any()) 
        {
            await GenerateMenuAsync();
        }
        var permissions =  _systemService.GeneratePermission(menus);
        if (permissions.Any()) 
        {
            await _context.Menus.AddRangeAsync(permissions);
            await _context.SaveChangesAsync();

            menus = await _context.Menus.ToListAsync();
            foreach (var menu in menus)
            {
                await _context.RoleMenus.AddAsync(new RoleMenu()
                {
                    Id = _snowFlakeService.GenerateId(),
                    MenuId = menu.Id,
                    RoleId = (long)administratorRoleId!
                });
            }
            await _context.SaveChangesAsync();

            _logger.LogInformation($"新增权限成功");
        }
    }

    public async Task GenerateMenuAsync() 
    {
        var json = "[{\"name\":\"home\",\"path\":\"/home\",\"meta\":{\"title\":\"首页\",\"icon\":\"el-icon-eleme-filled\",\"type\":\"menu\"},\"children\":[{\"name\":\"dashboard\",\"path\":\"/dashboard\",\"meta\":{\"title\":\"控制台\",\"icon\":\"el-icon-menu\",\"affix\":true},\"component\":\"home\"},{\"name\":\"userCenter\",\"path\":\"/usercenter\",\"meta\":{\"title\":\"帐号信息\",\"icon\":\"el-icon-user\",\"tag\":\"NEW\"},\"component\":\"userCenter\"}]},{\"name\":\"vab\",\"path\":\"/vab\",\"meta\":{\"title\":\"组件\",\"icon\":\"el-icon-takeaway-box\",\"type\":\"menu\"},\"children\":[{\"path\":\"/vab/mini\",\"name\":\"minivab\",\"meta\":{\"title\":\"原子组件\",\"icon\":\"el-icon-magic-stick\",\"type\":\"menu\"},\"component\":\"vab/mini\"},{\"path\":\"/vab/iconfont\",\"name\":\"iconfont\",\"meta\":{\"title\":\"扩展图标\",\"icon\":\"el-icon-orange\",\"type\":\"menu\"},\"component\":\"vab/iconfont\"},{\"path\":\"/vab/data\",\"name\":\"vabdata\",\"meta\":{\"title\":\"Data 数据展示\",\"icon\":\"el-icon-histogram\",\"type\":\"menu\"},\"children\":[{\"path\":\"/vab/chart\",\"name\":\"chart\",\"meta\":{\"title\":\"图表 Echarts\",\"type\":\"menu\"},\"component\":\"vab/chart\"},{\"path\":\"/vab/statistic\",\"name\":\"statistic\",\"meta\":{\"title\":\"统计数值\",\"type\":\"menu\"},\"component\":\"vab/statistic\"},{\"path\":\"/vab/video\",\"name\":\"scvideo\",\"meta\":{\"title\":\"视频播放器\",\"type\":\"menu\"},\"component\":\"vab/video\"},{\"path\":\"/vab/qrcode\",\"name\":\"qrcode\",\"meta\":{\"title\":\"二维码\",\"type\":\"menu\"},\"component\":\"vab/qrcode\"}]},{\"path\":\"/vab/form\",\"name\":\"vabform\",\"meta\":{\"title\":\"Form 数据录入\",\"icon\":\"el-icon-edit\",\"type\":\"menu\"},\"children\":[{\"path\":\"/vab/tableselect\",\"name\":\"tableselect\",\"meta\":{\"title\":\"表格选择器\",\"type\":\"menu\"},\"component\":\"vab/tableselect\"},{\"path\":\"/vab/formtable\",\"name\":\"formtable\",\"meta\":{\"title\":\"表单表格\",\"type\":\"menu\"},\"component\":\"vab/formtable\"},{\"path\":\"/vab/selectFilter\",\"name\":\"selectFilter\",\"meta\":{\"title\":\"分类筛选器\",\"type\":\"menu\"},\"component\":\"vab/selectFilter\"},{\"path\":\"/vab/filterbar\",\"name\":\"filterBar\",\"meta\":{\"title\":\"过滤器v2\",\"type\":\"menu\"},\"component\":\"vab/filterBar\"},{\"path\":\"/vab/upload\",\"name\":\"upload\",\"meta\":{\"title\":\"上传\",\"type\":\"menu\"},\"component\":\"vab/upload\"},{\"path\":\"/vab/select\",\"name\":\"scselect\",\"meta\":{\"title\":\"异步选择器\",\"type\":\"menu\"},\"component\":\"vab/select\"},{\"path\":\"/vab/iconselect\",\"name\":\"iconSelect\",\"meta\":{\"title\":\"图标选择器\",\"type\":\"menu\"},\"component\":\"vab/iconselect\"},{\"path\":\"/vab/cron\",\"name\":\"cron\",\"meta\":{\"title\":\"Cron规则生成器\",\"type\":\"menu\"},\"component\":\"vab/cron\"},{\"path\":\"/vab/editor\",\"name\":\"editor\",\"meta\":{\"title\":\"富文本编辑器\",\"type\":\"menu\"},\"component\":\"vab/editor\"},{\"path\":\"/vab/codeeditor\",\"name\":\"codeeditor\",\"meta\":{\"title\":\"代码编辑器\",\"type\":\"menu\"},\"component\":\"vab/codeeditor\"}]},{\"path\":\"/vab/feedback\",\"name\":\"vabfeedback\",\"meta\":{\"title\":\"Feedback 反馈\",\"icon\":\"el-icon-mouse\",\"type\":\"menu\"},\"children\":[{\"path\":\"/vab/drag\",\"name\":\"drag\",\"meta\":{\"title\":\"拖拽排序\",\"type\":\"menu\"},\"component\":\"vab/drag\"},{\"path\":\"/vab/contextmenu\",\"name\":\"contextmenu\",\"meta\":{\"title\":\"右键菜单\",\"type\":\"menu\"},\"component\":\"vab/contextmenu\"},{\"path\":\"/vab/cropper\",\"name\":\"cropper\",\"meta\":{\"title\":\"图像剪裁\",\"type\":\"menu\"},\"component\":\"vab/cropper\"},{\"path\":\"/vab/fileselect\",\"name\":\"fileselect\",\"meta\":{\"title\":\"资源库选择器(弃用)\",\"type\":\"menu\"},\"component\":\"vab/fileselect\"},{\"path\":\"/vab/dialog\",\"name\":\"dialogExtend\",\"meta\":{\"title\":\"弹窗扩展\",\"type\":\"menu\"},\"component\":\"vab/dialog\"}]},{\"path\":\"/vab/others\",\"name\":\"vabothers\",\"meta\":{\"title\":\"Others 其他\",\"icon\":\"el-icon-more-filled\",\"type\":\"menu\"},\"children\":[{\"path\":\"/vab/print\",\"name\":\"print\",\"meta\":{\"title\":\"打印\",\"type\":\"menu\"},\"component\":\"vab/print\"},{\"path\":\"/vab/watermark\",\"name\":\"watermark\",\"meta\":{\"title\":\"水印\",\"type\":\"menu\"},\"component\":\"vab/watermark\"},{\"path\":\"/vab/importexport\",\"name\":\"importexport\",\"meta\":{\"title\":\"文件导出导入\",\"type\":\"menu\"},\"component\":\"vab/importexport\"}]},{\"path\":\"/vab/list\",\"name\":\"list\",\"meta\":{\"title\":\"Table 数据列表\",\"icon\":\"el-icon-fold\",\"type\":\"menu\"},\"children\":[{\"path\":\"/vab/table/base\",\"name\":\"tableBase\",\"meta\":{\"title\":\"基础数据列表\",\"type\":\"menu\"},\"component\":\"vab/table/base\"},{\"path\":\"/vab/table/thead\",\"name\":\"tableThead\",\"meta\":{\"title\":\"多级表头\",\"type\":\"menu\"},\"component\":\"vab/table/thead\"},{\"path\":\"/vab/table/column\",\"name\":\"tableCustomColumn\",\"meta\":{\"title\":\"动态列\",\"type\":\"menu\"},\"component\":\"vab/table/column\"},{\"path\":\"/vab/table/remote\",\"name\":\"tableRemote\",\"meta\":{\"title\":\"远程排序过滤\",\"type\":\"menu\"},\"component\":\"vab/table/remote\"}]},{\"path\":\"/vab/workflow\",\"name\":\"workflow\",\"meta\":{\"title\":\"工作流设计器\",\"icon\":\"el-icon-share\",\"type\":\"menu\"},\"component\":\"vab/workflow\"},{\"path\":\"/vab/formrender\",\"name\":\"formRender\",\"meta\":{\"title\":\"动态表单(Beta)\",\"icon\":\"el-icon-message-box\",\"type\":\"menu\"},\"component\":\"vab/form\"}]},{\"name\":\"template\",\"path\":\"/template\",\"meta\":{\"title\":\"模板\",\"icon\":\"el-icon-files\",\"type\":\"menu\"},\"children\":[{\"path\":\"/template/layout\",\"name\":\"layoutTemplate\",\"meta\":{\"title\":\"布局\",\"icon\":\"el-icon-grid\",\"type\":\"menu\"},\"children\":[{\"path\":\"/template/layout/blank\",\"name\":\"blank\",\"meta\":{\"title\":\"空白模板\",\"type\":\"menu\"},\"component\":\"template/layout/blank\"},{\"path\":\"/template/layout/layoutTCB\",\"name\":\"layoutTCB\",\"meta\":{\"title\":\"上中下布局\",\"type\":\"menu\"},\"component\":\"template/layout/layoutTCB\"},{\"path\":\"/template/layout/layoutLCR\",\"name\":\"layoutLCR\",\"meta\":{\"title\":\"左中右布局\",\"type\":\"menu\"},\"component\":\"template/layout/layoutLCR\"}]},{\"path\":\"/template/list\",\"name\":\"list\",\"meta\":{\"title\":\"列表\",\"icon\":\"el-icon-document\",\"type\":\"menu\"},\"children\":[{\"path\":\"/template/list/crud\",\"name\":\"listCrud\",\"meta\":{\"title\":\"CRUD\",\"type\":\"menu\"},\"component\":\"template/list/crud\",\"children\":[{\"path\":\"/template/list/crud/detail/:id?\",\"name\":\"listCrud-detail\",\"meta\":{\"title\":\"新增/编辑\",\"hidden\":true,\"active\":\"/template/list/crud\",\"type\":\"menu\"},\"component\":\"template/list/crud/detail\"}]},{\"path\":\"/template/list/tree\",\"name\":\"listTree\",\"meta\":{\"title\":\"左树右表\",\"type\":\"menu\"},\"component\":\"template/list/tree\"},{\"path\":\"/template/list/tab\",\"name\":\"listTab\",\"meta\":{\"title\":\"分类表格\",\"type\":\"menu\"},\"component\":\"template/list/tab\"},{\"path\":\"/template/list/son\",\"name\":\"listSon\",\"meta\":{\"title\":\"子母表\",\"type\":\"menu\"},\"component\":\"template/list/son\"},{\"path\":\"/template/list/widthlist\",\"name\":\"widthlist\",\"meta\":{\"title\":\"定宽列表\",\"type\":\"menu\"},\"component\":\"template/list/width\"}]},{\"path\":\"/template/other\",\"name\":\"other\",\"meta\":{\"title\":\"其他\",\"icon\":\"el-icon-folder\",\"type\":\"menu\"},\"children\":[{\"path\":\"/template/other/stepform\",\"name\":\"stepform\",\"meta\":{\"title\":\"分步表单\",\"type\":\"menu\"},\"component\":\"template/other/stepform\"}]}]},{\"name\":\"other\",\"path\":\"/other\",\"meta\":{\"title\":\"其他\",\"icon\":\"el-icon-more-filled\",\"type\":\"menu\"},\"children\":[{\"path\":\"/other/directive\",\"name\":\"directive\",\"meta\":{\"title\":\"指令\",\"icon\":\"el-icon-price-tag\",\"type\":\"menu\"},\"component\":\"other/directive\"},{\"path\":\"/other/viewTags\",\"name\":\"viewTags\",\"meta\":{\"title\":\"标签操作\",\"icon\":\"el-icon-files\",\"type\":\"menu\"},\"component\":\"other/viewTags\",\"children\":[{\"path\":\"/other/fullpage\",\"name\":\"fullpage\",\"meta\":{\"title\":\"整页路由\",\"icon\":\"el-icon-monitor\",\"fullpage\":true,\"hidden\":true,\"type\":\"menu\"},\"component\":\"other/fullpage\"}]},{\"path\":\"/other/verificate\",\"name\":\"verificate\",\"meta\":{\"title\":\"表单验证\",\"icon\":\"el-icon-open\",\"type\":\"menu\"},\"component\":\"other/verificate\"},{\"path\":\"/other/loadJS\",\"name\":\"loadJS\",\"meta\":{\"title\":\"异步加载JS\",\"icon\":\"el-icon-location-information\",\"type\":\"menu\"},\"component\":\"other/loadJS\"},{\"path\":\"/link\",\"name\":\"link\",\"meta\":{\"title\":\"外部链接\",\"icon\":\"el-icon-link\",\"type\":\"menu\"},\"children\":[{\"path\":\"https://baidu.com\",\"name\":\"百度\",\"meta\":{\"title\":\"百度\",\"type\":\"link\"}},{\"path\":\"https://www.google.cn\",\"name\":\"谷歌\",\"meta\":{\"title\":\"谷歌\",\"type\":\"link\"}}]},{\"path\":\"/iframe\",\"name\":\"Iframe\",\"meta\":{\"title\":\"Iframe\",\"icon\":\"el-icon-position\",\"type\":\"menu\"},\"children\":[{\"path\":\"https://v3.cn.vuejs.org\",\"name\":\"vue3\",\"meta\":{\"title\":\"VUE 3\",\"type\":\"iframe\"}},{\"path\":\"https://element-plus.gitee.io\",\"name\":\"elementplus\",\"meta\":{\"title\":\"Element Plus\",\"type\":\"iframe\"}},{\"path\":\"https://lolicode.gitee.io/scui-doc\",\"name\":\"scuidoc\",\"meta\":{\"title\":\"SCUI文档\",\"type\":\"iframe\"}}]}]},{\"name\":\"test\",\"path\":\"/test\",\"meta\":{\"title\":\"实验室\",\"icon\":\"el-icon-mouse\",\"type\":\"menu\"},\"children\":[{\"path\":\"/test/autocode\",\"name\":\"autocode\",\"meta\":{\"title\":\"代码生成器\",\"icon\":\"sc-icon-code\",\"type\":\"menu\"},\"component\":\"test/autocode/index\",\"children\":[{\"path\":\"/test/autocode/table\",\"name\":\"autocode-table\",\"meta\":{\"title\":\"CRUD代码生成\",\"hidden\":true,\"active\":\"/test/autocode\",\"type\":\"menu\"},\"component\":\"test/autocode/table\"}]},{\"path\":\"/test/codebug\",\"name\":\"codebug\",\"meta\":{\"title\":\"异常处理\",\"icon\":\"sc-icon-bug-line\",\"type\":\"menu\"},\"component\":\"test/codebug\"}]},{\"name\":\"setting\",\"path\":\"/setting\",\"meta\":{\"title\":\"配置\",\"icon\":\"el-icon-setting\",\"type\":\"menu\"},\"children\":[{\"path\":\"/setting/system\",\"name\":\"system\",\"meta\":{\"title\":\"系统设置\",\"icon\":\"el-icon-tools\",\"type\":\"menu\"},\"component\":\"setting/system\"},{\"path\":\"/setting/user\",\"name\":\"user\",\"meta\":{\"title\":\"用户管理\",\"icon\":\"el-icon-user-filled\",\"type\":\"menu\"},\"component\":\"setting/user\"},{\"path\":\"/setting/role\",\"name\":\"role\",\"meta\":{\"title\":\"角色管理\",\"icon\":\"el-icon-notebook\",\"type\":\"menu\"},\"component\":\"setting/role\"},{\"path\":\"/setting/dept\",\"name\":\"dept\",\"meta\":{\"title\":\"部门管理\",\"icon\":\"sc-icon-organization\",\"type\":\"menu\"},\"component\":\"setting/dept\"},{\"path\":\"/setting/dic\",\"name\":\"dic\",\"meta\":{\"title\":\"字典管理\",\"icon\":\"el-icon-document\",\"type\":\"menu\"},\"component\":\"setting/dic\"},{\"path\":\"/setting/table\",\"name\":\"tableSetting\",\"meta\":{\"title\":\"表格列管理\",\"icon\":\"el-icon-scale-to-original\",\"type\":\"menu\"},\"component\":\"setting/table\"},{\"path\":\"/setting/menu\",\"name\":\"settingMenu\",\"meta\":{\"title\":\"菜单管理\",\"icon\":\"el-icon-fold\",\"type\":\"menu\"},\"component\":\"setting/menu\"},{\"path\":\"/setting/task\",\"name\":\"task\",\"meta\":{\"title\":\"计划任务\",\"icon\":\"el-icon-alarm-clock\",\"type\":\"menu\"},\"component\":\"setting/task\"},{\"path\":\"/setting/client\",\"name\":\"client\",\"meta\":{\"title\":\"应用管理\",\"icon\":\"el-icon-help-filled\",\"type\":\"menu\"},\"component\":\"setting/client\"},{\"path\":\"/setting/log\",\"name\":\"log\",\"meta\":{\"title\":\"系统日志\",\"icon\":\"el-icon-warning\",\"type\":\"menu\"},\"component\":\"setting/log\"}]},{\"path\":\"/other/about\",\"name\":\"about\",\"meta\":{\"title\":\"关于\",\"icon\":\"el-icon-info-filled\",\"type\":\"menu\"},\"component\":\"other/about\"}]";

        JsonSerializerOptions options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true // 启用大小写不敏感的属性名称匹配
        };
        var routeConfig = JsonSerializer.Deserialize<RouteConfig[]>(json);

        foreach (var item in routeConfig)
        {
            var routes = RouteConfigHelper.FlattenAndAssignIds(item);
            foreach (var route in routes)
            {
                _context.Menus.Add(new Menu
                {
                    Id = (route.parentId == null ? route.id : _snowFlakeService.GenerateId()),
                    ParentId = route.parentId,
                    Name = route.name,
                    Path = route.path,
                    Component = route.component,
                    Meta = new Domain.ValueObjects.Meta(Domain.Enums.MetaType.Menu, route.meta.title, route.meta.icon, null, null, null, null, null, null)
                });
                await _context.SaveChangesAsync();
            }
        }
    }

    public class RouteConfig
    {
        public long id { get; set; }
        public long? parentId { get; set; }
        public string name { get; set; }
        public string path { get; set; }
        public string component { get; set; }
        public RouteMeta meta { get; set; }
        public List<RouteConfig> children { get; set; }
    }

    public class RouteMeta
    {
        public string title { get; set; }
        public string icon { get; set; }
        public string type { get; set; }
    }
}

public static class RouteConfigHelper
{
    public static List<RouteConfig> FlattenAndAssignIds(RouteConfig root)
    {
        AssignIds(root, null); // 初始节点的父节点id为null
        return Flatten(root);
    }

    private static void AssignIds(RouteConfig node, long? parentId)
    {
        node.id = new SnowFlakeService(1, 1).GenerateId(); // 分配唯一的id
        node.parentId = parentId;

        if (node.children != null)
        {
            foreach (var child in node.children)
            {
                AssignIds(child, node.id); // 分配id给子节点，并将当前节点的id作为父节点id传递
            }
        }
    }

    private static List<RouteConfig> Flatten(RouteConfig node)
    {
        var result = new List<RouteConfig> { node };

        if (node.children != null)
        {
            foreach (var child in node.children)
            {
                result.AddRange(Flatten(child));
            }
        }

        return result;
    }
}