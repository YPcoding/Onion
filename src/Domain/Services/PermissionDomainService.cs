using Domain.Entities;
using Domain.Repositories;

namespace Domain.Services;

public class PermissionDomainService
{
    private readonly IPermissionRepository _repository;

    public PermissionDomainService(
        IPermissionRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Permission>> GetPermissionsByUserIdAsync(long userId) 
    {
        return await _repository.GetPermissionsByUserIdAsync(userId);
    }
}
