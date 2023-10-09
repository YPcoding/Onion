using System.ComponentModel.DataAnnotations;
using Domain.Entities.Departments;
using Application.Features.Departments.Caching;
using Domain.Entities;
using Microsoft.Extensions.Options;

namespace Application.Features.Departments.Commands.Add;

/// <summary>
/// 添加
/// </summary>
[Map(typeof(Department))]
[Description("新增部门")]
public class AddDepartmentCommand : IRequest<Result<long>>
{
    /// <summary>
    /// 部门名称
    /// </summary>
    [Description("部门名称")]
    [Required(ErrorMessage = "部门名称是必填的")]
    public string DepartmentName { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [Description("排序")]
    [Required(ErrorMessage = "排序是必填的")]
    public int Sort { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    [Description("描述")]
    [Required(ErrorMessage = "描述是必填的")]
    public string Description { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    [Description("状态")]
    [Required(ErrorMessage = "状态是必填的")]
    public bool IsActive { get; set; }

    /// <summary>
    /// 上级节点
    /// </summary>
    [Description("上级节点")]
    public long? SuperiorId { get; set; }
}
/// <summary>
/// 处理程序
/// </summary>
public class AddDepartmentCommandHandler : IRequestHandler<AddDepartmentCommand, Result<long>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public AddDepartmentCommandHandler(
        IApplicationDbContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// 业务逻辑
    /// </summary>
    /// <param name="request">请求参数</param>
    /// <param name="cancellationToken">取消标记</param>
    /// <returns>返回处理结果</returns>
    public async Task<Result<long>> Handle(AddDepartmentCommand request, CancellationToken cancellationToken)
    {
        var department = _mapper.Map<Department>(request);
        //department.AddDomainEvent(new CreatedEvent<Department>(department));
        await _context.Departments.AddAsync(department);
        var isSuccess = await _context.SaveChangesAsync(cancellationToken) > 0;
        return await Result<long>.SuccessOrFailureAsync(department.Id, isSuccess, new string[] { "操作失败" });
    }
}