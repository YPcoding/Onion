using Ardalis.Specification;
using Domain.Entities.Identity;

namespace Application.Features.Menus.Specifications;

public class MenuByIdSpec : Specification<Menu>
{
    public MenuByIdSpec(long id)
    {
        Query.Where(q => q.Id == id);
    }
}
