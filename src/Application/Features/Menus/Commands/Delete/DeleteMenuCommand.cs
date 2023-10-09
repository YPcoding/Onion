namespace Application.Features.Menus.Commands.Delete;

/// <summary>
/// 删除菜单管理
/// </summary>
[Description("删除菜单")]
public class DeleteMenuCommand : IRequest<Result<bool>>
{
    /// <summary>
    /// 唯一标识
    /// </summary>
    [Description("唯一标识")]
    public List<long> MenuIds { get; set; }
}

/// <summary>
/// 处理程序
/// </summary>
public class DeleteMenuCommandHandler : IRequestHandler<DeleteMenuCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public DeleteMenuCommandHandler(
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
    public async Task<Result<bool>> Handle(DeleteMenuCommand request, CancellationToken cancellationToken)
    {
        var menusToDelete = await _context.Menus
            .Where(x => request.MenuIds.Contains(x.Id))
            .ToListAsync();

        if (menusToDelete.Any())
        {
            _context.Menus.RemoveRange(menusToDelete);
            var isSuccess = await _context.SaveChangesAsync(cancellationToken) > 0;
            return await Result<bool>.SuccessOrFailureAsync(isSuccess, isSuccess, new string[] { "操作失败" });
        }

        return await Result<bool>.FailureAsync(new string[] { "没有找到需要删除的数据" });
    }
}