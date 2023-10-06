using Ardalis.Specification;
using Domain.Entities.Departments;

namespace Application.Features.Departments.Specifications;

public class DepartmentByIdSpec : Specification<Department>
{
    public DepartmentByIdSpec(long id)
    {
        Query.Where(q => q.Id == id);
    }
}
