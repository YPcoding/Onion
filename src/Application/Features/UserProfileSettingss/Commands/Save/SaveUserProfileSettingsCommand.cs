using Application.Features.Roles.Caching;
using Application.Features.UserProfileSettings.Caching;
using Domain.Entities.Settings;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.UserProfileSettings.Commands.Save;

/// <summary>
/// 保存个人设置
/// </summary>
[Map(typeof(UserProfileSetting))]
[Description("保存个人设置")]
public class SaveUserProfileSettingsCommand : ICacheInvalidatorRequest<Result<bool>>
{
    /// <summary>
    /// 设置名称
    /// </summary>
    [Description("设置名称")]
    [Required(ErrorMessage = "设置名称必填")]
    public string SettingName { get; set; }

    /// <summary>
    /// 设置相关值
    /// </summary>
    [Description("设置相关值")]
    public string SettingValue { get; set; }

    /// <summary>
    /// 相关值类型
    /// </summary>
    [Description("相关值类型")]
    [Required(ErrorMessage = "相关值类型必填")]
    public string ValueType { get; set; }

    /// <summary>
    /// 默认值
    /// </summary>
    [Description("默认值必填")]
    [Required(ErrorMessage = "默认值必填")]
    public string DefaultValue { get; set; }

    [JsonIgnore]
    public string CacheKey => UserProfileSettingCacheKey.GetAllCacheKey;
    [JsonIgnore]
    public CancellationTokenSource? SharedExpiryTokenSource => UserProfileSettingCacheKey.SharedExpiryTokenSource();
}
/// <summary>
/// 处理程序
/// </summary>
public class SaveUserProfileSettingsCommandHandler : IRequestHandler<SaveUserProfileSettingsCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public SaveUserProfileSettingsCommandHandler(
        IApplicationDbContext context,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// 业务逻辑
    /// </summary>
    /// <param name="request">请求参数</param>
    /// <param name="cancellationToken">取消标记</param>
    /// <returns>返回处理结果</returns>
    public async Task<Result<bool>> Handle(SaveUserProfileSettingsCommand request, CancellationToken cancellationToken)
    {
        var userprofilesetting = await _context.UserProfileSettings
            .FirstOrDefaultAsync(x => x.UserId == _currentUserService.CurrentUserId && x.SettingName == request.SettingName);
        if (userprofilesetting == null)
        {
            userprofilesetting = _mapper.Map<UserProfileSetting>(request);
            userprofilesetting.UserId = _currentUserService.CurrentUserId;
            await _context.UserProfileSettings.AddAsync(userprofilesetting);
        }
        else if (userprofilesetting != null && !request.SettingValue.IsNullOrWhiteSpace())
        {
            userprofilesetting = _mapper.Map(request, userprofilesetting);
            userprofilesetting.UserId = _currentUserService.CurrentUserId;
            _context.UserProfileSettings.Update(userprofilesetting);
        }
        else
        {
            _context.UserProfileSettings.Remove(userprofilesetting!);
        }
        var isSuccess = await _context.SaveChangesAsync(cancellationToken) > 0;
        return await Result<bool>.SuccessOrFailureAsync(isSuccess, isSuccess, new string[] { "操作失败" });
    }
}