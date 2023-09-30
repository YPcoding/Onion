using Domain.Entities.Identity;
using Domain.Repositories;

namespace Infrastructure.Repositories;

/// <summary>
/// 用户仓储
/// </summary>
public class UserRepository : IUserRepository, IScopedDependency
{
    private readonly IApplicationDbContext _dbContext;

    public UserRepository(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User> AccessFailedAsync(User user)
    {
        user.LoginFailedIfExceedConntWillBeLock();
        await UpdateAsync(user);
        return user;
    }

    public async Task<(User?, string? password)> AddAdminUserAsync(string userName, string phoneNum)
    {
        var user = await FindByNameAsync(userName);
        if (user != null) throw new Exception("用户名已存在");
        var phone = await FindByPhoneNumberAsync(phoneNum);
        if (phone != null) throw new Exception("手机号码已存在");

        var password = "123456";
        user = new User
        {
            UserName = userName,
            PhoneNumber = phoneNum
        };
        user.PasswordHash = user.CreatePassword(password);
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        return (user, password);
    }

    public Task<bool> AddToRoleAsync(User user, string role)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> ChangePasswordAsync(long userId, string password)
    {
        var user = await FindByIdAsync(userId);
        if (user == null) return false;
        user.ChangePassword(password);
        return await UpdateAsync(user);
    }

    /// <summary>
    /// 检查登录信息
    /// </summary>
    /// <param name="user">用户信息</param>
    /// <param name="password">密码</param>
    /// <param name="lockoutOnFailure">是否记录登录失败次数</param>
    /// <returns></returns>
    public async Task<bool> CheckForSignInAsync(User user, string password, bool lockoutOnFailure)
    {
        if (user.CompareWithOldPassword(password)) return true;
        if (lockoutOnFailure) await AccessFailedAsync(user);
        return false;
    }

    public async Task ConfirmPhoneNumberAsync(long userId)
    {
        var user = await FindByIdAsync(userId);
        user!.PhoneNumberConfirmed = true;
        await UpdateAsync(user);
    }

    public async Task<User> CreateAsync(User user, string password)
    {
        user.PasswordHash = user.CreatePassword(password);
        await UpdateAsync(user);
        return user;
    }

    public async Task<User?> FindByIdAsync(long userId)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
    }

    public async Task<User?> FindByNameAsync(string userName)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(x => x.UserName == userName);
    }

    public async Task<User?> FindByPhoneNumberAsync(string phoneNum)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNum);
    }

    public Task<IList<string>> GetRolesAsync(User user)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> RemoveUserAsync(long userId)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
        _dbContext.Users.Remove(user!);
        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<(User?, string? password)> ResetPasswordAsync(long userId)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
        if (user == null) return (null, null);
        var password = "123456";
        user.PasswordHash = user.CreatePassword(password);
        _dbContext.Users.Update(user);
        if (await _dbContext.SaveChangesAsync() > 0)
            return (user, password);

        throw new Exception("重置密码失败");
    }

    public async Task<bool> UpdatePhoneNumberAsync(long userId, string phoneNum)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
        if (user == null) return false;
        user.PhoneNumber = phoneNum;
        _dbContext.Users.Update(user);
        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateUserAvatarUri(User user)
    {
        return await UpdateAsync(user);
    }

    public async Task<bool> UpdateAsync(User user) 
    {
        _dbContext.Users.Update(user);
        return await _dbContext.SaveChangesAsync() > 0;
    }
}
