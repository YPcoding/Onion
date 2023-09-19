using Ardalis.Specification;
using Domain.Entities;

namespace Application.Features.Users.Specifications;

public class RoleByIdSpec : Specification<Role>
{
    public RoleByIdSpec(long id)
    {
        Query.Where(q => q.Id == id);
    }
}
