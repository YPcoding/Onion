using Application.Common.Extensions;
using Application.Features.Users.Caching;
using Application.Features.Users.DTOs;
using Application.Features.Users.Specifications;
using AutoMapper.QueryableExtensions;
using Domain.Repositories;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.Users.Queries.GetById;

/// <summary>
/// 通过用户唯一标识获取一条数据
/// </summary>
[Description("查询单条用户数据")]
public class GetUserByIdQuery : ICacheableRequest<Result<UserDto>>
{
    /// <summary>
    /// 唯一标识
    /// </summary>
    [Required(ErrorMessage = "唯一标识必填的")]
    public long UserId { get; set; }
    [JsonIgnore]
    public string CacheKey => UserCacheKey.GetByIdCacheKey(UserId);
    [JsonIgnore]
    public MemoryCacheEntryOptions? Options => UserCacheKey.MemoryCacheEntryOptions;
}

/// <summary>
/// 处理程序
/// </summary>
public class GetUserByIdQueryHandler :IRequestHandler<GetUserByIdQuery, Result<UserDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IRoleRepository _roleRepository;
    private readonly IMapper _mapper;

    public GetUserByIdQueryHandler(
        IApplicationDbContext context,
        IMapper mapper
,
        IRoleRepository roleRepository)
    {
        _context = context;
        _mapper = mapper;
        _roleRepository = roleRepository;
    }

    /// <summary>
    /// 业务逻辑
    /// </summary>
    /// <param name="request">请求参数</param>
    /// <param name="cancellationToken">取消标记</param>
    /// <returns>返回一条查询的用户数据</returns>
    /// <exception cref="NotFoundException">未找到数据移除处理</exception>
    public async Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Include(ur=>ur.UserRoles)
            .ThenInclude(r=>r.Role)
            .ApplySpecification(new UserByIdSpec(request.UserId))
            .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync(cancellationToken) ?? throw new NotFoundException($"用户唯一标识: [{request.UserId}] 未找到");

        var userRoles = await _roleRepository
           .GetUserRolesAsync(r => r.UserRoles.Any(ur => user.Id ==ur.UserId));

        if (userRoles!.Any())
        {
            var roles = userRoles!.Where(x => x.UserRoles.Any(u => u.UserId == user.Id));
            if (roles.Any())
            {
                user.Roles = _mapper.Map(roles, user.Roles);
            }
        }

        return await Result<UserDto>.SuccessAsync(user);
    }
}