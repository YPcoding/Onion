using Application.Features.UserProfileSettings.Caching;
using Application.Features.UserProfileSettings.DTOs;
using AutoMapper.QueryableExtensions;

namespace Application.Features.UserProfileSettings.Queries.GetAll;

[Description("查询所有个人设置")]
public class GetAllUserProfileSettingsQuery : ICacheableRequest<Result<IEnumerable<UserProfileSettingsDto>>>
{
    [JsonIgnore]
    public string CacheKey => UserProfileSettingCacheKey.GetAllCacheKey;
    [JsonIgnore]
    public MemoryCacheEntryOptions? Options => UserProfileSettingCacheKey.MemoryCacheEntryOptions;
}

[Description("查询个人设置")]
public class GetUserProfileSettingsQueryByUserId : ICacheableRequest<Result<IEnumerable<UserProfileSettingsDto>>>
{
    public required long UserId { get; set; }

    [JsonIgnore]
    public string CacheKey => UserProfileSettingCacheKey.GetByUserIdCacheKey(UserId);
    [JsonIgnore]
    public MemoryCacheEntryOptions? Options => UserProfileSettingCacheKey.MemoryCacheEntryOptions;
}

public class GetAllUserProfileSettingsQueryHandler :
     IRequestHandler<GetAllUserProfileSettingsQuery, Result<IEnumerable<UserProfileSettingsDto>>>,
     IRequestHandler<GetUserProfileSettingsQueryByUserId, Result<IEnumerable<UserProfileSettingsDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAllUserProfileSettingsQueryHandler(
        IApplicationDbContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<UserProfileSettingsDto>>> Handle(GetAllUserProfileSettingsQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.UserProfileSettings
            .ProjectTo<UserProfileSettingsDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
        return await Result<IEnumerable<UserProfileSettingsDto>>.SuccessAsync(data);
    }

    public async Task<Result<IEnumerable<UserProfileSettingsDto>>> Handle(GetUserProfileSettingsQueryByUserId request, CancellationToken cancellationToken)
    {
        var data = await _context.UserProfileSettings
            .Where(x=>x.UserId== request.UserId)
            .ProjectTo<UserProfileSettingsDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
        return await Result<IEnumerable<UserProfileSettingsDto>>.SuccessAsync(data);
    }
}

