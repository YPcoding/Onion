using Domain.Entities.Settings;

namespace Application.Features.UserProfileSettings.Commands.Add;

/// <summary>
/// 添加个人设置
/// </summary>
[Map(typeof(UserProfileSetting))]
[Description("新增个人设置")]
public class AddUserProfileSettingsCommand : IRequest<Result<long>>
{        
        /// <summary>
        /// 设置名称
        /// </summary>
        [Description("设置名称")]
        public string SettingName { get; set; }
        
        /// <summary>
        /// 设置相关值
        /// </summary>
        [Description("设置相关值")]
        public string SettingValue { get; set; }
}
/// <summary>
/// 处理程序
/// </summary>
public class AddUserProfileSettingsCommandHandler : IRequestHandler<AddUserProfileSettingsCommand, Result<long>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public AddUserProfileSettingsCommandHandler(
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
    public async Task<Result<long>> Handle(AddUserProfileSettingsCommand request, CancellationToken cancellationToken)
    {
        var userprofilesetting = _mapper.Map<UserProfileSetting>(request);
        //userprofilesettings.AddDomainEvent(new CreatedEvent<UserProfileSettings>(userprofilesettings));
        await _context.UserProfileSettings.AddAsync(userprofilesetting);
        var isSuccess = await _context.SaveChangesAsync(cancellationToken) > 0;
        return await Result<long>.SuccessOrFailureAsync(userprofilesetting.Id, isSuccess, new string[] { "操作失败" });
    }
}