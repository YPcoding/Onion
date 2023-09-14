using Ardalis.Specification;
using Masuit.Tools;

namespace Application.Features.Users.Specifications;

public class UserAdvancedSpecification : Specification<User>
{
    public UserAdvancedSpecification(UserAdvancedFilter filter)
    {
        Query.Where(x => x.UserName == filter.UserName, !filter.UserName.IsNullOrEmpty())
             .Where(x => x.Email == filter.Email, !filter.Email.IsNullOrEmpty())
             .Where(x => x.PhoneNumber == filter.PhoneNumber, !filter.PhoneNumber.IsNullOrEmpty())
             .Where(x => x.EmailConfirmed == filter.EmailConfirmed, filter.EmailConfirmed.HasValue)
             .Where(x => x.LockoutEnabled == filter.LockoutEnabled, filter.LockoutEnabled.HasValue);
    }
}