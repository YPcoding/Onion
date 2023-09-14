using System.ComponentModel.DataAnnotations;

namespace Application.Features.Users.Commands.Delete;

/// <summary>
/// 删除用户
/// </summary>
public class DeleteUserCommand : IRequest<Result<bool>>
{
    /// <summary>
    /// 主键
    /// </summary>
    [Required]
    public List<long> Ids { get; set; }
}

/// <summary>
/// 处理程序
/// </summary>
public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public DeleteUserCommandHandler(
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
    public async Task<Result<bool>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var items = await _context.Users.Where(x=> request.Ids.Contains(x.Id)).ToListAsync();
        _context.Users.RemoveRange(items);
        var isSuccess = await _context.SaveChangesAsync(cancellationToken) > 0;
        return await Result<bool>.SuccessOrFailureAsync(
            isSuccess,
            isSuccess,
            new string[] { "操作失败" });
    }
}