﻿using System.ComponentModel.DataAnnotations;

namespace Application.Features.Users.Commands.AddEdit;

/// <summary>
/// 添加用户
/// </summary>
[Map(typeof(User))]
public class AddUserCommand : IRequest<Result<long>>
{
    /// <summary>
    /// 用户名
    /// </summary>
    [Required]
    public string UserName { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    [Required]
    public string Password { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 手机号码
    /// </summary>
    public string? PhoneNumber { get; set; }
}

/// <summary>
/// 处理程序
/// </summary>
public class AddUserCommandHandler : IRequestHandler<AddUserCommand, Result<long>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public AddUserCommandHandler(
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
    public async Task<Result<long>> Handle(AddUserCommand request, CancellationToken cancellationToken)
    {
        var user = _mapper.Map<User>(request);
        user.PasswordHash = request.Password.MDString3("Onion");
        user.AddDomainEvent(new CreatedEvent<User>(user));
        _context.Users.Add(user);
        var isSuccess = await _context.SaveChangesAsync(cancellationToken) > 0;
        return await Result<long>.SuccessOrFailureAsync(
            user.Id,
            isSuccess,
            new string[] { "操作失败"});
    }
}
