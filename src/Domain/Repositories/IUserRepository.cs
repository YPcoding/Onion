namespace Domain.Repositories;

/// <summary>
/// 用户仓储
/// </summary>
public interface IUserRepository : IScopedDependency
{
    /// <summary>
    /// 根据Id获取用户
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns></returns>
    Task<User?> FindByIdAsync(long userId);

    /// <summary>
    /// 根据用户名获取用户
    /// </summary>
    /// <param name="userName"></param>
    /// <returns></returns>
    Task<User?> FindByNameAsync(string userName);

    /// <summary>
    /// 根据手机号获取用户
    /// </summary>
    /// <param name="phoneNum"></param>
    /// <returns></returns>
    Task<User?> FindByPhoneNumberAsync(string phoneNum);

    /// <summary>
    /// 创建用户
    /// </summary>
    /// <param name="user"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    Task<User> CreateAsync(User user, string password);

    /// <summary>
    /// 记录一次登陆失败
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    Task<User> AccessFailedAsync(User user);

    /// <summary>
    /// 修改密码
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="password">密码</param>
    /// <returns></returns>
    Task<bool> ChangePasswordAsync(long userId, string password);

    /// <summary>
    /// 获取用户的角色
    /// </summary>
    /// <param name="user">用户信息</param>
    /// <returns></returns>
    Task<IList<string>> GetRolesAsync(User user);

    /// <summary>
    /// 把用户user加入角色role
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="role">角色</param>
    /// <returns></returns>
    Task<bool> AddToRoleAsync(User user, string role);

    /// <summary>
    /// 为了登录而检查用户名、密码是否正确
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="password">密码</param>
    /// <param name="lockoutOnFailure">如果登录失败，则记录一次登陆失败</param>
    /// <returns></returns>
    public Task<bool> CheckForSignInAsync(User user, string password, bool lockoutOnFailure);

    /// <summary>
    /// 确认手机号
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public Task ConfirmPhoneNumberAsync(long userId);

    /// <summary>
    /// 修改手机号
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="phoneNum"></param>
    /// <returns></returns>
    public Task<bool> UpdatePhoneNumberAsync(long userId, string phoneNum);

    /// <summary>
    /// 删除用户
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public Task<bool> RemoveUserAsync(long userId);

    /// <summary>
    /// 添加管理员
    /// </summary>
    /// <param name="userName">用户名</param>
    /// <param name="phoneNum">手机号码</param>
    /// <returns>返回值第三个是生成的密码</returns>
    public Task<(User?, string? password)> AddAdminUserAsync(string userName, string phoneNum);

    /// <summary>
    /// 重置密码
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>返回值第三个是生成的密码</returns>
    public Task<(User?, string? password)> ResetPasswordAsync(long userId);

    /// <summary>
    /// 修改用户头像
    /// </summary>
    /// <param name="user">用户信息</param>
    /// <returns></returns>
    public Task<bool> UpdateUserAvatarUri(User user);

    /// <summary>
    /// 修改用户
    /// </summary>
    /// <param name="user">用户信息</param>
    /// <returns></returns>
    public Task<bool> UpdateAsync(User user);
}
