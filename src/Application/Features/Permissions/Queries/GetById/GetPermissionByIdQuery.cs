using Application.Features.Permissions.Caching;
using Application.Features.Permissions.DTOs;
using AutoMapper.QueryableExtensions;

namespace Application.Features.Permissions.Queries.GetById;

/// <summary>
/// 通过权限唯一标识获取一条数据
/// </summary>
public class GetPermissionByIdQuery : ICacheableRequest<Result<PermissionDto>>
{
    /// <summary>
    /// 权限唯一标识
    /// </summary>
    public required long PermissionId { get; set; }
    [JsonIgnore]
    public string CacheKey => PermissionCacheKey.GetByIdCacheKey(PermissionId);
    [JsonIgnore]
    public MemoryCacheEntryOptions? Options => PermissionCacheKey.MemoryCacheEntryOptions;
}

/// <summary>
/// 处理程序
/// </summary>
public class GetPermissionByIdQueryHandler :
     IRequestHandler<GetPermissionByIdQuery, Result<PermissionDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetPermissionByIdQueryHandler(
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
    /// <returns>返回单个权限数据</returns>
    public async Task<Result<PermissionDto>> Handle(GetPermissionByIdQuery request, CancellationToken cancellationToken)
    {
        var permission = await _context.Permissions
            .ProjectTo<PermissionDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(x => x.Id == request.PermissionId, cancellationToken);
        return await Result<PermissionDto>.SuccessAsync(permission!);
    }
}
