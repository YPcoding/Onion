using Application.Features.Menus.DTOs;
using Domain.ValueObjects;

namespace Application.Features.Menus.Commands.AddEdit;

/// <summary>
/// 添加菜单管理
/// </summary>
[Map(typeof(Menu))]
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
    public Meta Meta { get; set; }

    /// <summary>
    /// 视图
    /// </summary>
    [Description("视图")]
    public string? Component { get; set; }

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
    private readonly IMapper _mapper;

    public AddMenuCommandHandler(
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
    public async Task<Result<long>> Handle(AddEditMenuCommand request, CancellationToken cancellationToken)
    {
        var dto = _mapper.Map<MenuDto>(request);

        if (request.Id.HasValue)
        {
            var menu = await _context.Menus.FindAsync(new object[] { request.Id }, cancellationToken);
            _ = menu ?? throw new NotFoundException($"菜单唯一标识{request.Id}未找到。");
            menu = _mapper.Map(dto, menu);
            menu.AddDomainEvent(new UpdatedEvent<Menu>(menu));
            await _context.SaveChangesAsync(cancellationToken);
            return await Result<long>.SuccessAsync(menu.Id);
        }
        else
        {
            var menu = _mapper.Map<Menu>(dto);
            menu.AddDomainEvent(new UpdatedEvent<Menu>(menu));
            _context.Menus.Add(menu);
            await _context.SaveChangesAsync(cancellationToken);
            return await Result<long>.SuccessAsync(menu.Id);
        }
    }
}