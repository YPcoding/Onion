using Application.Common.Extensions;
using Domain.Entities.Settings;
using Application.Features.UserProfileSettings.Caching;
using Application.Features.UserProfileSettings.DTOs;
using Application.Features.UserProfileSettings.Specifications;

namespace Application.Features.UserProfileSettings.Queries.Pagination;

/// <summary>
/// 个人设置分页查询
/// </summary>
[Description("分页查询个人设置")]
public class UserProfileSettingsWithPaginationQuery : UserProfileSettingsAdvancedFilter, IRequest<Result<PaginatedData<UserProfileSettingsDto>>>
{
    [JsonIgnore]
    public UserProfileSettingsAdvancedPaginationSpec Specification => new UserProfileSettingsAdvancedPaginationSpec(this);
}

/// <summary>
/// 处理程序
/// </summary>
public class UserProfileSettingsWithPaginationQueryHandler :
    IRequestHandler<UserProfileSettingsWithPaginationQuery, Result<PaginatedData<UserProfileSettingsDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    public UserProfileSettingsWithPaginationQueryHandler(
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
    /// <returns>返回个人设置分页数据</returns>
    public async Task<Result<PaginatedData<UserProfileSettingsDto>>> Handle(
        UserProfileSettingsWithPaginationQuery request,
        CancellationToken cancellationToken)
    {
        var userprofilesettings = await _context.UserProfileSettings
            .OrderBy($"{request.OrderBy} {request.SortDirection}")
            .ProjectToPaginatedDataAsync<UserProfileSetting, UserProfileSettingsDto>(
            request.Specification,
            request.PageNumber,
            request.PageSize,
            _mapper.ConfigurationProvider,
            cancellationToken);

        return await Result<PaginatedData<UserProfileSettingsDto>>.SuccessAsync(userprofilesettings);
    }
}
