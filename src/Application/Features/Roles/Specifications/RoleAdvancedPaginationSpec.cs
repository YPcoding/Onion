using Ardalis.Specification;

namespace Application.Features.Users.Specifications;

public class RoleAdvancedPaginationSpec : Specification<Role>
{
    public RoleAdvancedPaginationSpec(RoleAdvancedFilter filter)
    {
        Query.Where(q => q.RoleName!.Contains(filter.Keyword!) || q.Description!.Contains(filter.Keyword!), !string.IsNullOrEmpty(filter.Keyword))
             .Where(x => x.RoleName == filter.RoleName, !filter.RoleName!.IsNullOrEmpty())
             .Where(x => x.RoleCode == filter.RoleCode, !filter.RoleCode!.IsNullOrEmpty())
             .Where(x => x.IsActive == filter.IsActive, filter.IsActive.HasValue);
    }
}