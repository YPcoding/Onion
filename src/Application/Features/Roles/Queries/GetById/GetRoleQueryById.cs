using Application.Common.Extensions;
using Application.Features.Roles.Caching;
using Application.Features.Roles.DTOs;
using Application.Features.Users.Specifications;
using AutoMapper.QueryableExtensions;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.Roles.Queries.GetById;

/// <summary>
/// 通过角色唯一标识获取一条数据
/// </summary>
[Description("获取单条角色数据")]
public class GetRoleQueryById : ICacheableRequest<Result<RoleDto>>
{
    /// <summary>
    /// 唯一标识
    /// </summary>
    [Required(ErrorMessage = "唯一标识必填的")]
    public long RoleId { get; set; }
    [JsonIgnore]
    public string CacheKey => RoleCacheKey.GetByIdCacheKey(RoleId);
    [JsonIgnore]
    public MemoryCacheEntryOptions? Options => RoleCacheKey.MemoryCacheEntryOptions;
}

/// <summary>
/// 处理程序
/// </summary>
public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleQueryById, Result<RoleDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetRoleByIdQueryHandler(
        IApplicationDbContext context,
        IMapper mapper
        )
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// 业务逻辑
    /// </summary>
    /// <param name="request">请求参数</param>
    /// <param name="cancellationToken">取消标记</param>
    /// <returns>返回一条查询的角色数据</returns>
    /// <exception cref="NotFoundException">未找到数据移除处理</exception>
    public async Task<Result<RoleDto>> Handle(GetRoleQueryById request, CancellationToken cancellationToken)
    {
        var role = await _context.Roles.ApplySpecification(new RoleByIdSpec(request.RoleId))
                     .ProjectTo<RoleDto>(_mapper.ConfigurationProvider)
                     .SingleOrDefaultAsync(cancellationToken) ?? throw new NotFoundException($"角色唯一标识: [{request.RoleId}] 未找到");
        return  await Result<RoleDto>.SuccessAsync(role);
    }
}
