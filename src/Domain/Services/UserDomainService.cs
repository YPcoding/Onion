using Domain.Repositories;

namespace Domain.Services;

public class UserDomainService : IScopedDependency
{
    private readonly IUserRepository _repository;

    public UserDomainService(IUserRepository repository)
    {
        _repository = repository;
    }
    private async Task<bool> CheckUserNameAndPasswordAsync(string userName, string password)
    {
        var user = await _repository.FindByNameAsync(userName);
        if (user == null)
        {
            return false;
        }

        return await _repository.CheckForSignInAsync(user, password, true);
    }

    private async Task<bool> CheckPhoneNumberAndPasswordAsync(string phoneNumber, string password)
    {
        var user = await _repository.FindByPhoneNumberAsync(phoneNumber);
        if (user == null)
        {
            return false;
        }
        return await _repository.CheckForSignInAsync(user, password, true);
    }

    public async Task<User?> LoginByPhoneAndPasswordAsync(string phoneNumber, string password)
    {
        var succeeded = await CheckPhoneNumberAndPasswordAsync(phoneNumber, password);
        if (!succeeded) return null;
        var user = await _repository.FindByPhoneNumberAsync(phoneNumber);
        if (user == null) return null;
        return user;
    }

    public async Task<User?> LoginByUserNameAndPasswordAsync(string userName, string password)
    {
        var succeeded = await CheckUserNameAndPasswordAsync(userName, password);
        if (!succeeded) return null;
        var user = await _repository.FindByNameAsync(userName);
        if (user == null) return null;
        return user;
    }

    public async Task<bool> CheckUserIsLockoutEnabledAsync(string userName)
    {
        var user = await _repository.FindByNameAsync(userName);
        return user?.LockoutEnabled ?? true;
    }
}
