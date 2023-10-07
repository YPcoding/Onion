using Ardalis.Specification;

namespace Application.Features.Users.Specifications;

public class UserAdvancedPaginationSpec : Specification<User>
{
    public UserAdvancedPaginationSpec(UserAdvancedFilter filter)
    {
        Query.Where(x => x.UserName!.Contains(filter.Keyword!) || x.Realname!.Contains(filter.Keyword!), !filter.Keyword!.IsNullOrWhiteSpace())
             //.Where(x => x.UserName == filter.UserName, !filter.UserName!.IsNullOrWhiteSpace())
             .Where(x => x.Email == filter.Email, !filter.Email!.IsNullOrWhiteSpace())
             .Where(x => x.PhoneNumber == filter.PhoneNumber, !filter.PhoneNumber!.IsNullOrWhiteSpace())
             .Where(x=>x.DepartmentId == filter.DepartmentId, filter.DepartmentId.HasValue)
             .Where(x => x.EmailConfirmed == filter.EmailConfirmed, filter.EmailConfirmed.HasValue)
             .Where(x => x.LockoutEnabled == filter.LockoutEnabled, filter.LockoutEnabled.HasValue);
    }
}