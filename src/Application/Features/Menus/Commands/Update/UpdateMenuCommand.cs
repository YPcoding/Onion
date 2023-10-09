using Domain.ValueObjects;

namespace Application.Features.Menus.Commands.Update;


/// <summary>
/// 修改菜单管理
/// </summary>
[Map(typeof(Menu))]
[Description("修改菜单")]
public class UpdateMenuCommand : IRequest<Result<long>>
{

        
        /// <summary>
        /// 父级节点
        /// </summary>
        [Description("父级节点")]
        public long? ParentId { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [Description("")]
        public Menu Parent { get; set; }
        
        /// <summary>
        /// 名称
        /// </summary>
        [Description("名称")]
        public string Name { get; set; }
        
        /// <summary>
        /// 路径
        /// </summary>
        [Description("路径")]
        public string Path { get; set; }
        
        /// <summary>
        /// 重定向
        /// </summary>
        [Description("重定向")]
        public string Redirect { get; set; }
        
        /// <summary>
        /// 菜单高亮
        /// </summary>
        [Description("菜单高亮")]
        public string Active { get; set; }
        
        /// <summary>
        /// 元信息
        /// </summary>
        [Description("元信息")]
        public Meta Meta { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [Description("")]
        public List<Menu> Children { get; set; }
        
        /// <summary>
        /// 唯一标识
        /// </summary>
        [Description("唯一标识")]
        public long MenuId { get; set; }
        
        /// <summary>
        /// 乐观并发标记
        /// </summary>
        [Description("乐观并发标记")]
        public string ConcurrencyStamp { get; set; }
}

/// <summary>
/// 处理程序
/// </summary>
public class UpdateMenuCommandHandler : IRequestHandler<UpdateMenuCommand, Result<long>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UpdateMenuCommandHandler(
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
    public async Task<Result<long>> Handle(UpdateMenuCommand request, CancellationToken cancellationToken)
    {
        var menu = await _context.Menus
           .SingleOrDefaultAsync(x => x.Id == request.MenuId, cancellationToken)
           ?? throw new NotFoundException($"数据【{request.MenuId}】未找到");

        menu = _mapper.Map(request, menu);
        //menu.AddDomainEvent(new UpdatedEvent<Menu>(menu));
        _context.Menus.Update(menu);
        var isSuccess = await _context.SaveChangesAsync(cancellationToken) > 0;
        return await Result<long>.SuccessOrFailureAsync(menu.Id, isSuccess, new string[] { "操作失败" });
    }
}
