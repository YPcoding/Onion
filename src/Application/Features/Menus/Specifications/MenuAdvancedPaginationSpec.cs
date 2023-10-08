using Ardalis.Specification;
using Domain.Entities.Identity;


namespace Application.Features.Menus.Specifications;

public class MenuAdvancedPaginationSpec : Specification<Menu>
{
    public MenuAdvancedPaginationSpec(MenuAdvancedFilter filter)
    {
        Query     
            .Where(x => x.Name == filter.Name, !filter.Name.IsNullOrEmpty())
     
            .Where(x => x.Path == filter.Path, !filter.Path.IsNullOrEmpty())
     
            .Where(x => x.Redirect == filter.Redirect, !filter.Redirect.IsNullOrEmpty())
     
            .Where(x => x.Active == filter.Active, !filter.Active.IsNullOrEmpty())
;    }
}