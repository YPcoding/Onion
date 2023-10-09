namespace Application.Features.Departments.Commands.Delete;

/// <summary>
/// 删除
/// </summary>
[Description("删除部门")]
public class DeleteDepartmentCommand : IRequest<Result<bool>>
{
  
        /// <summary>
        /// 唯一标识
        /// </summary>
        [Description("唯一标识")]
        public List<long> DepartmentIds { get; set; }
}

/// <summary>
/// 处理程序
/// </summary>
public class DeleteDepartmentCommandHandler : IRequestHandler<DeleteDepartmentCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public DeleteDepartmentCommandHandler(
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
    public async Task<Result<bool>> Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
    {
        var departmentsToDelete = await _context.Departments
            .Where(x => request.DepartmentIds.Contains(x.Id))
            .ToListAsync();

        if (departmentsToDelete.Any())
        {
            _context.Departments.RemoveRange(departmentsToDelete);
            var isSuccess = await _context.SaveChangesAsync(cancellationToken) > 0;
            return await Result<bool>.SuccessOrFailureAsync(isSuccess, isSuccess,new string[] {"操作失败"});
        }

        return await Result<bool>.FailureAsync(new string[] { "没有找到需要删除的数据" });
    }
}