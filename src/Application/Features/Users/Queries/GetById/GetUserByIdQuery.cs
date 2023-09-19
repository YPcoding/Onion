using Application.Common.Extensions;
using Application.Features.Users.Caching;
using Application.Features.Users.DTOs;
using Application.Features.Users.Specifications;
using AutoMapper.QueryableExtensions;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.Users.Queries.GetById;

/// <summary>
/// 通过用户唯一标识获取一条数据
/// </summary>
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
    private readonly IMapper _mapper;

    public GetUserByIdQueryHandler(
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
    /// <returns>返回一条查询的用户数据</returns>
    /// <exception cref="NotFoundException">未找到数据移除处理</exception>
    public async Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.ApplySpecification(new UserByIdSpec(request.UserId))
                     .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
                     .FirstAsync(cancellationToken) ?? throw new NotFoundException($"用户唯一标识: [{request.UserId}] 未找到");
        return await Result<UserDto>.SuccessAsync(user);
    }
}