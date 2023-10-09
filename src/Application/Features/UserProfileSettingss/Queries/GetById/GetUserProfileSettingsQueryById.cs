using Application.Common.Extensions;
using Domain.Entities.Settings;
using Application.Features.UserProfileSettings.Caching;
using Application.Features.UserProfileSettings.DTOs;
using Application.Features.UserProfileSettings.Specifications;
using AutoMapper.QueryableExtensions;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.UserProfileSettings.Queries.GetById;

/// <summary>
/// 通过唯一标识获取一条数据
/// </summary>
[Description("查询单条个人设置")]
public class GetUserProfileSettingsQueryById : IRequest<Result<UserProfileSettingsDto>>
{
    /// <summary>
    /// 唯一标识
    /// </summary>
    [Required(ErrorMessage = "唯一标识必填的")]
    public long UserProfileSettingsId { get; set; }
}

/// <summary>
/// 处理程序
/// </summary>
public class GetUserProfileSettingsByIdQueryHandler :IRequestHandler<GetUserProfileSettingsQueryById, Result<UserProfileSettingsDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetUserProfileSettingsByIdQueryHandler(
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
    /// <returns>返回查询的一条数据</returns>
    /// <exception cref="NotFoundException">未找到数据移除处理</exception>
    public async Task<Result<UserProfileSettingsDto>> Handle(GetUserProfileSettingsQueryById request, CancellationToken cancellationToken)
    {
        var userprofilesettings = await _context.UserProfileSettings.ApplySpecification(new UserProfileSettingsByIdSpec(request.UserProfileSettingsId))
                     .ProjectTo<UserProfileSettingsDto>(_mapper.ConfigurationProvider)
                     .SingleOrDefaultAsync(cancellationToken) ?? throw new NotFoundException($"唯一标识: [{request.UserProfileSettingsId}] 未找到");
        return await Result<UserProfileSettingsDto>.SuccessAsync(userprofilesettings);
    }
}
