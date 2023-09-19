using Ardalis.Specification;

namespace Application.Features.Users.Specifications;

public class UserByIdSpec : Specification<User>
{
    public UserByIdSpec(long id)
    {
        Query.Where(q => q.Id == id);
    }
}
