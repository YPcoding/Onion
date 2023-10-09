using Application.Features.UserProfileSettings.Caching;
using Domain.Entities.Settings;
using Domain.Entities;

namespace Application.Features.UserProfileSettings.Commands.Delete;

/// <summary>
/// 删除个人设置
/// </summary>
[Description("删除个人设置")]
public class DeleteUserProfileSettingsCommand : IRequest<Result<bool>>
{
  
        /// <summary>
        /// 唯一标识
        /// </summary>
        [Description("唯一标识")]
        public List<long> UserProfileSettingsIds { get; set; }
}

/// <summary>
/// 处理程序
/// </summary>
public class DeleteUserProfileSettingsCommandHandler : IRequestHandler<DeleteUserProfileSettingsCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public DeleteUserProfileSettingsCommandHandler(
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
    public async Task<Result<bool>> Handle(DeleteUserProfileSettingsCommand request, CancellationToken cancellationToken)
    {
        var UserProfileSettingsToDelete = await _context.UserProfileSettings
            .Where(x => request.UserProfileSettingsIds.Contains(x.Id))
            .ToListAsync();

        if (UserProfileSettingsToDelete.Any())
        {
            _context.UserProfileSettings.RemoveRange(UserProfileSettingsToDelete);
            var isSuccess = await _context.SaveChangesAsync(cancellationToken) > 0;
            return await Result<bool>.SuccessOrFailureAsync(isSuccess, isSuccess,new string[] {"操作失败"});
        }

        return await Result<bool>.FailureAsync(new string[] { "没有找到需要删除的数据" });
    }
}