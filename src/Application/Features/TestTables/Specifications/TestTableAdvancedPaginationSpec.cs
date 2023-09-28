using Ardalis.Specification;
using Masuit.Tools;

namespace Application.Features.TestTables.Specifications;

public class TestTableAdvancedPaginationSpec : Specification<TestTable>
{
    public TestTableAdvancedPaginationSpec(TestTableAdvancedFilter filter)
    {
        Query
            .Where(x => x.Name == filter.Name, !filter.Name.IsNullOrEmpty())
            .Where(x => x.Description == filter.Description, !filter.Description.IsNullOrEmpty())
            .Where(x => x.Stuts == filter.Stuts, filter.Stuts.HasValue);
    }
}