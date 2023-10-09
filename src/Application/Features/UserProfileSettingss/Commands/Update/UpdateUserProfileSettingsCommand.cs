using Application.Features.UserProfileSettings.Caching;
using Domain.Entities.Settings;
using Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.UserProfileSettings.Commands.Update;


/// <summary>
/// 修改个人设置
/// </summary>
[Map(typeof(UserProfileSetting))]
[Description("修改个人设置")]
public class UpdateUserProfileSettingsCommand : IRequest<Result<long>>
{

        
        /// <summary>
        /// 用户
        /// </summary>
        [Description("用户")]
        public User User { get; set; }
        
        /// <summary>
        /// 用户唯一标识
        /// </summary>
        [Description("用户唯一标识")]
        public long UserId { get; set; }
        
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
        
        /// <summary>
        /// 唯一标识
        /// </summary>
        [Description("唯一标识")]
        public long UserProfileSettingsId { get; set; }
        
        /// <summary>
        /// 乐观并发标记
        /// </summary>
        [Description("乐观并发标记")]
        public string ConcurrencyStamp { get; set; }
}

/// <summary>
/// 处理程序
/// </summary>
public class UpdateUserProfileSettingsCommandHandler : IRequestHandler<UpdateUserProfileSettingsCommand, Result<long>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UpdateUserProfileSettingsCommandHandler(
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
    public async Task<Result<long>> Handle(UpdateUserProfileSettingsCommand request, CancellationToken cancellationToken)
    {
        var userprofilesettings = await _context.UserProfileSettings
           .SingleOrDefaultAsync(x => x.Id == request.UserProfileSettingsId, cancellationToken)
           ?? throw new NotFoundException($"数据【{request.UserProfileSettingsId}】未找到");

        userprofilesettings = _mapper.Map(request, userprofilesettings);
        //userprofilesettings.AddDomainEvent(new UpdatedEvent<UserProfileSettings>(userprofilesettings));
        _context.UserProfileSettings.Update(userprofilesettings);
        var isSuccess = await _context.SaveChangesAsync(cancellationToken) > 0;
        return await Result<long>.SuccessOrFailureAsync(userprofilesettings.Id, isSuccess, new string[] { "操作失败" });
    }
}
