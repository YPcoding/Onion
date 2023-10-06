using Ardalis.Specification;
using Domain.Entities.Departments;


namespace Application.Features.Departments.Specifications;

public class DepartmentAdvancedPaginationSpec : Specification<Department>
{
    public DepartmentAdvancedPaginationSpec(DepartmentAdvancedFilter filter)
    {
        Query     
            .Where(x => x.DepartmentName == filter.DepartmentName, !filter.DepartmentName.IsNullOrEmpty())
     
            .Where(x => x.Description == filter.Description, !filter.Description.IsNullOrEmpty())
;    }
}