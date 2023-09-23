using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class Permission : BaseAuditableEntity
{
    public Permission() 
    {
        RolePermissions = new HashSet<RolePermission>();
    }

    public Permission(long? superiorId, string label, string code, PermissionType type, string path, string icon, bool hidden, bool enabled, bool closable, bool opened, bool newWindow, bool external, int sort, string description)
    {
        SuperiorId = superiorId;
        Label = label;
        Code = code;
        Type = type;
        CreatePath(path);
        Icon = icon;
        Hidden = hidden;
        Enabled = enabled;
        Closable = closable;
        Opened = opened;
        NewWindow = newWindow;
        External = external;
        Sort = sort;
        Description = description;
    }

    /// <summary>
    /// 自动添加权限
    /// </summary>
    /// <param name="group">分组名称</param>
    /// <param name="label">标签名称</param>
    /// <param name="path">接口路径</param>
    /// <param name="sort">排序</param>
    /// <param name="httpMethods">请求方法</param>
    /// <param name="type">权限类型</param>
    /// <param name="description">描述</param>
    /// <param name="icon">Icon</param>
    public Permission(string group, string label, string path, int sort, string httpMethods, PermissionType type,string description,string icon)
    {
        Group = group;
        Label = label;
        CreateCode(path);
        Type = type;
        CreatePath(path);
        Sort = sort;
        Description = description;
        HttpMethods = httpMethods;
        Icon = icon;
    }

    /// <summary>
    /// 上级节点
    /// </summary>
    public long? SuperiorId { get; set; }
    [ForeignKey("SuperiorId")]
    public Permission? Superior { get; set; } = null;

    public ICollection<RolePermission> RolePermissions { get; set; }
    /// <summary>
    /// 权限名称
    /// </summary>
    public string? Label { get; set; }

    /// <summary>
    /// 权限编码
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// 权限类型
    /// 菜  单 10
    /// 页  面 20
    /// 权限点 30
    /// </summary>
    public PermissionType Type { get; set; }

    /// <summary>
    /// 菜单访问地址
    /// </summary>
    public string? Path { get; set; }

    /// <summary>
    /// 接口提交方法
    /// </summary>
    public string? HttpMethods { get; set; }

    /// <summary>
    /// 图标
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// 隐藏
    /// </summary>
    public bool? Hidden { get; set; } = false;

    /// <summary>
    /// 启用
    /// </summary>
    public bool? Enabled { get; set; } = true;

    /// <summary>
    /// 可关闭
    /// </summary>
    public bool? Closable { get; set; }

    /// <summary>
    /// 打开组
    /// </summary>
    public bool? Opened { get; set; }

    /// <summary>
    /// 打开新窗口
    /// </summary>
    public bool? NewWindow { get; set; }

    /// <summary>
    /// 链接外显
    /// </summary>
    public bool? External { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int? Sort { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 分组名称
    /// </summary>
    public string? Group { get; set; }

    /// <summary>
    /// 添加菜单
    /// </summary>
    public Permission AddMenu(int superiorId, string menuName, string path, string icon, bool hidden, bool enabled, bool external, int sort, string description) 
    {
        this.SuperiorId = superiorId;
        this.Label = menuName;
        this.Icon = icon;
        this.Hidden = hidden;
        this.Enabled = enabled;
        this.External = external;
        this.Sort = sort;
        this.Description = description;
        this.Type = PermissionType.Menu;
        CreatePath(path);
        CreateCode(path);
        return this;
    }

    /// <summary>
    /// 添加页面
    /// </summary>
    public Permission AddPage(int superiorId, string pageName, string path, string icon, bool hidden, bool enabled, bool closable, bool newWindow, bool external, int sort, string description)
    {
        this.SuperiorId = superiorId;
        this.Label = pageName;
        CreatePath(path);
        CreateCode(path);
        this.Icon = icon;
        this.Hidden = hidden;
        this.Enabled = enabled;
        this.Closable = closable;
        this.NewWindow = newWindow;
        this.External = external;
        this.Sort = sort;
        this.Description = description;
        this.Type = PermissionType.Page;
        return this;
    }

    /// <summary>
    /// 添加权限点
    /// </summary>
    public Permission AddDot(long superiorId, string dotName, string path, bool enabled, int sort, string description, string? httpMethods = null)
    {
        SuperiorId = superiorId;
        Label = dotName;
        Type = PermissionType.Dot;
        CreatePath(path);
        Enabled = enabled;
        Sort = sort;
        Description = description;
        HttpMethods = httpMethods;
        CreateCode(path);

        return this;
    }
    
    /// <summary>
    /// 创建权限编码
    /// </summary>
    /// <param name="path">接口路径</param>
    /// <returns></returns>
    public void CreateCode(string path) 
    {
        Code = path?.Replace("/", ":").ToLower() ?? "";
        if (Code.StartsWith(":"))
            Code = Code.Substring(1);
    }

    /// <summary>
    /// 创建PATH
    /// </summary>
    /// <param name="path"></param>
    public void CreatePath(string path) 
    {
        if (path.Contains('{'))
        {
            string originalString = path;

            // 找到最后一个 '/' 的索引位置
            int lastIndex = originalString.LastIndexOf('/');
            if (lastIndex >= 0)
            {
                // 使用 .Substring() 方法截取字符串
                string result = originalString.Substring(0, lastIndex);
                path = result;
            }
        }

        if (path.StartsWith("/"))
            Path = path;
        else
            Path = $"/{path}";
    }
}
