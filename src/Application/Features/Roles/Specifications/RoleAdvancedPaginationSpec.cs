using Ardalis.Specification;
using Domain.Entities;
using Masuit.Tools;

namespace Application.Features.Users.Specifications;

public class RoleAdvancedPaginationSpec : Specification<Role>
{
    public RoleAdvancedPaginationSpec(RoleAdvancedFilter filter)
    {
        Query.Where(q => q.RoleName!.Contains(filter.Keyword) || q.Description!.Contains(filter.Keyword), !string.IsNullOrEmpty(filter.Keyword))
             .Where(x => x.RoleName == filter.RoleName, !filter.RoleName.IsNullOrEmpty())
             .Where(x => x.Description == filter.Description, !filter.Description.IsNullOrEmpty());
    }
}