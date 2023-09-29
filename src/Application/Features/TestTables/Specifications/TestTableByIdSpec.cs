using Ardalis.Specification;
using Domain.Entities;

namespace Application.Features.TestTables.Specifications;

public class TestTableByIdSpec : Specification<TestTable>
{
    public TestTableByIdSpec(long id)
    {
        Query.Where(q => q.Id == id);
    }
}
