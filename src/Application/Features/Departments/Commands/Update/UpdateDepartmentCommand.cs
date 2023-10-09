using Domain.Entities.Departments;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.Departments.Commands.Update;


/// <summary>
/// 修改
/// </summary>
[Map(typeof(Department))]
[Description("修改部门")]
public class UpdateDepartmentCommand : IRequest<Result<long>>
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

    /// <summary>
    /// 唯一标识
    /// </summary>
    [Description("唯一标识")]
    [Required(ErrorMessage = "唯一标识是必填的")]
    public long DepartmentId { get; set; }

    /// <summary>
    /// 乐观并发标记
    /// </summary>
    [Description("乐观并发标记")]
    [Required(ErrorMessage = "乐观并发标记是必填的")]
    public string ConcurrencyStamp { get; set; }
}

/// <summary>
/// 处理程序
/// </summary>
public class UpdateDepartmentCommandHandler : IRequestHandler<UpdateDepartmentCommand, Result<long>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UpdateDepartmentCommandHandler(
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
    public async Task<Result<long>> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
    {
        var department = await _context.Departments
           .SingleOrDefaultAsync(x => x.Id == request.DepartmentId, cancellationToken)
           ?? throw new NotFoundException($"数据【{request.DepartmentId}】未找到");

        department = _mapper.Map(request, department);
        //department.AddDomainEvent(new UpdatedEvent<Department>(department));
        _context.Departments.Update(department);
        var isSuccess = await _context.SaveChangesAsync(cancellationToken) > 0;
        return await Result<long>.SuccessOrFailureAsync(department.Id, isSuccess, new string[] { "操作失败" });
    }
}
