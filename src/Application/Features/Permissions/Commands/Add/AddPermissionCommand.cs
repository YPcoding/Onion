using Application.Features.Permissions.Caching;
using Domain.Entities;
using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.Permissions.Commands.Add;

/// <summary>
/// 添加权限
/// </summary>
[Map(typeof(Permission))]
public class AddPermissionCommand : ICacheInvalidatorRequest<Result<long>>
{
    /// <summary>
    /// 父级节点
    /// </summary>
    public long? SuperiorId { get; set; }

    /// <summary>
    /// 权限名称
    /// </summary>
    [Required(ErrorMessage = "权限名称是必填的")]
    public string Label { get; set; }

    /// <summary>
    /// 权限类型；菜单：10；页面：0；权限点：30
    /// </summary>
    [Required(ErrorMessage = "权限类型是必填的")]
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
    /// 缓存Key值
    /// </summary>
    [JsonIgnore]
    public string CacheKey => PermissionCacheKey.GetAllCacheKey;

    /// <summary>
    /// 取消令牌源
    /// </summary>
    [JsonIgnore]
    public CancellationTokenSource? SharedExpiryTokenSource => PermissionCacheKey.SharedExpiryTokenSource();
}

/// <summary>
/// 处理程序
/// </summary>
public class AddPermissionCommandHandler : IRequestHandler<AddPermissionCommand, Result<long>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public AddPermissionCommandHandler(
        IApplicationDbContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// 业务逻辑
    /// </summary>
    /// <param name="request">请求参数</param>
    /// <param name="cancellationToken">取消标记</param>
    /// <returns>返回处理结果</returns>
    public async Task<Result<long>> Handle(AddPermissionCommand request, CancellationToken cancellationToken)
    {
        var permission = _mapper.Map<Permission>(request);
        permission.CreatePath(request.Path!);
        permission.CreateCode(request.Path!);
        permission.AddDomainEvent(new CreatedEvent<Permission>(permission));    
        
        await _context.Permissions.AddAsync(permission);
        var isSuccess = await _context.SaveChangesAsync(cancellationToken) > 0;

        return await Result<long>.SuccessOrFailureAsync(permission.Id, isSuccess, new string[] { "操作失败" });
    }
}