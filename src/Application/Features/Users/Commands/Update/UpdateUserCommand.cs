using System.ComponentModel.DataAnnotations;

namespace Application.Features.Users.Commands.Update;

/// <summary>
/// 修改用户
/// </summary>
[Map(typeof(User))]
public class UpdateUserCommand : IRequest<Result<long>>
{
    /// <summary>
    /// 主键
    /// </summary>
    [Required]
    public long Id { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 手机号码
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// 并发标记
    /// </summary>
    [Required]
    public string ConcurrencyStamp { get; set; }
}

/// <summary>
/// 处理程序
/// </summary>
public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result<long>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UpdateUserCommandHandler(
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
    /// <exception cref="NotFoundException">未找到的异常处理</exception>
    public async Task<Result<long>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var item = await _context.Users
        .SingleOrDefaultAsync(x => x.Id == request.Id && x.ConcurrencyStamp == request.ConcurrencyStamp, cancellationToken) ??
                     throw new NotFoundException($"数据【{request.Id}-{request.ConcurrencyStamp}】未找到");

        item = _mapper.Map(request, item);
        item.AddDomainEvent(new UpdatedEvent<User>(item));
        _context.Users.Update(item);
        var isSuccess = await _context.SaveChangesAsync(cancellationToken) > 0;
        return await Result<long>.SuccessOrFailureAsync(
            request.Id,
            isSuccess,
            new string[] { "操作失败" });
    }
}