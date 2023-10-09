using Application.Common.Extensions;
using Domain.Entities.Departments;
using Application.Features.Departments.Caching;
using Application.Features.Departments.DTOs;
using Application.Features.Departments.Specifications;
using AutoMapper.QueryableExtensions;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.Departments.Queries.GetById;

/// <summary>
/// 通过唯一标识获取一条数据
/// </summary>
[Description("查询单条部门数据")]
public class GetDepartmentQueryById : IRequest<Result<DepartmentDto>>
{
    /// <summary>
    /// 唯一标识
    /// </summary>
    [Required(ErrorMessage = "唯一标识必填的")]
    public long DepartmentId { get; set; }
}

/// <summary>
/// 处理程序
/// </summary>
public class GetDepartmentByIdQueryHandler :IRequestHandler<GetDepartmentQueryById, Result<DepartmentDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetDepartmentByIdQueryHandler(
        IApplicationDbContext context,
        IMapper mapper
        )
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// 业务逻辑
    /// </summary>
    /// <param name="request">请求参数</param>
    /// <param name="cancellationToken">取消标记</param>
    /// <returns>返回查询的一条数据</returns>
    /// <exception cref="NotFoundException">未找到数据移除处理</exception>
    public async Task<Result<DepartmentDto>> Handle(GetDepartmentQueryById request, CancellationToken cancellationToken)
    {
        var department = await _context.Departments.ApplySpecification(new DepartmentByIdSpec(request.DepartmentId))
                     .ProjectTo<DepartmentDto>(_mapper.ConfigurationProvider)
                     .SingleOrDefaultAsync(cancellationToken) ?? throw new NotFoundException($"唯一标识: [{request.DepartmentId}] 未找到");
        return await Result<DepartmentDto>.SuccessAsync(department);
    }
}
