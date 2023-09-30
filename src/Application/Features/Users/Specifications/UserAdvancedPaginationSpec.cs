using Ardalis.Specification;


namespace Application.Features.Users.Specifications;

public class UserAdvancedPaginationSpec : Specification<User>
{
    public UserAdvancedPaginationSpec(UserAdvancedFilter filter)
    {
        Query.Where(x => x.UserName == filter.UserName, !filter.UserName.IsNullOrEmpty())
             .Where(x => x.Email == filter.Email, !filter.Email.IsNullOrEmpty())
             .Where(x => x.PhoneNumber == filter.PhoneNumber, !filter.PhoneNumber.IsNullOrEmpty())
             .Where(x => x.EmailConfirmed == filter.EmailConfirmed, filter.EmailConfirmed.HasValue)
             .Where(x => x.LockoutEnabled == filter.LockoutEnabled, filter.LockoutEnabled.HasValue);
    }
}