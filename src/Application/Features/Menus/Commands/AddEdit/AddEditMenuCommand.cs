using Application.Features.Menus.DTOs;
using Domain.ValueObjects;

namespace Application.Features.Menus.Commands.AddEdit;

/// <summary>
/// 保存菜单管理
/// </summary>
[Map(typeof(Menu))]
[Description("保存菜单")]
public class AddEditMenuCommand : IRequest<Result<long>>
{
    /// <summary>
    /// 唯一标识
    /// </summary>
    [Description("唯一标识")]
    public long? Id { get; set; }

    /// <summary>
    /// 父级节点
    /// </summary>
    [Description("父级节点")]
    public long? ParentId { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    [Description("名称")]
    public string? Name { get; set; }

    /// <summary>
    /// 路径
    /// </summary>
    [Description("路径")]
    public string? Path { get; set; }

    /// <summary>
    /// 重定向
    /// </summary>
    [Description("重定向")]
    public string? Redirect { get; set; }

    /// <summary>
    /// 菜单高亮
    /// </summary>
    [Description("菜单高亮")]
    public string? Active { get; set; }

    /// <summary>
    /// 元信息
    /// </summary>
    [Description("元信息")]
    public Meta? Meta { get; set; }

    /// <summary>
    /// 视图
    /// </summary>
    [Description("视图")]
    public string? Component { get; set; }

    /// <summary>
    /// 接口权限
    /// </summary>
    [Description("接口权限")]
    public List<Api>? ApiList { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<MenuDto, AddEditMenuCommand>().ReverseMap();
        }
    }
}
/// <summary>
/// 处理程序
/// </summary>
public class AddMenuCommandHandler : IRequestHandler<AddEditMenuCommand, Result<long>>
{
    private readonly IApplicationDbContext _context;
    private readonly ISnowFlakeService _snowflakeService;
    private readonly IMapper _mapper;

    public AddMenuCommandHandler(
        IApplicationDbContext context,
        IMapper mapper,
        ISnowFlakeService snowflakeService)
    {
        _context = context;
        _mapper = mapper;
        _snowflakeService = snowflakeService;
    }

    /// <summary>
    /// 业务逻辑
    /// </summary>
    /// <param name="request">请求参数</param>
    /// <param name="cancellationToken">取消标记</param>
    /// <returns>返回处理结果</returns>
    public async Task<Result<long>> Handle(AddEditMenuCommand request, CancellationToken cancellationToken)
    {
        const MetaType apiType = MetaType.Api;
        Menu? menuToUpdate = new();
        Menu menuToAdd = new();

        if (request.Id.HasValue)
        {
            menuToUpdate = await _context.Menus.FindAsync(new object[] { request.Id }, cancellationToken);
            _ = menuToUpdate ?? throw new NotFoundException($"菜单唯一标识{request.Id}未找到。");

            _mapper.Map(request, menuToUpdate);
            menuToUpdate.AddDomainEvent(new UpdatedEvent<Menu>(menuToUpdate));

            if (request.ApiList != null)
            {
                var apiToDeletes = await _context.Menus
                    .Where(x => x.ParentId == menuToUpdate.Id && x.Meta!.Type == apiType)
                    .ToListAsync();

                if (apiToDeletes.Any())
                {
                    _context.Menus.RemoveRange(apiToDeletes);
                }

                foreach (var item in request.ApiList)
                {
                    await _context.Menus.AddAsync(new Menu()
                    {
                        Id = _snowflakeService.GenerateId(),
                        ParentId = menuToUpdate.Id,
                        Code = item.Code,
                        Url = item.Url,
                        Meta = new Meta(apiType, null, null, null, null, null, null, null, null)
                    });
                }
            }
        }
        else
        {
            menuToAdd = _mapper.Map<Menu>(request);
            menuToAdd.AddDomainEvent(new UpdatedEvent<Menu>(menuToAdd));
            _context.Menus.Add(menuToAdd);

            if (request.ApiList != null)
            {
                foreach (var item in request.ApiList)
                {
                    await _context.Menus.AddAsync(new Menu()
                    {
                        Id = _snowflakeService.GenerateId(),
                        ParentId = menuToAdd.Id,
                        Code = item.Code,
                        Url = item.Url,
                        Meta = new Meta(apiType, null, null, null, null, null, null, null, null)
                    });
                }
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        if (request.Id.HasValue)
        {
            return await Result<long>.SuccessAsync(menuToUpdate.Id);
        }
        else
        {
            return await Result<long>.SuccessAsync(menuToAdd.Id);
        }
    }
}