using Application.Features.Departments.DTOs;
using AutoMapper.QueryableExtensions;

namespace Application.Features.Departments.Queries.GetAll;

[Description("获取所有部门")]
public class GetAllDepartmentsQuery : IRequest<Result<IEnumerable<DepartmentDto>>>
{
    /// <summary>
    /// 部门名称
    /// </summary>
    [Description("部门名称")]
    public string? DepartmentName { get; set; }
}

public class GetAllDepartmentsQueryHandler :
    IRequestHandler<GetAllDepartmentsQuery, Result<IEnumerable<DepartmentDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAllDepartmentsQueryHandler(
        IApplicationDbContext context,
        IMapper mapper
    )
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<DepartmentDto>>> Handle(GetAllDepartmentsQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.Departments
            .ProjectTo<DepartmentDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        if (data != null && data.Count > 0 && !request.DepartmentName.IsNullOrWhiteSpace()) 
        {
            data = data.Where(x=>x.DepartmentName.Contains(request.DepartmentName)).ToList();
        }

        return await Result<IEnumerable<DepartmentDto>>.SuccessAsync(data);
    }
}

