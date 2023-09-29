using Application.Features.TestTables.Caching;
using Domain.Entities;
using Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.TestTables.Commands.Update;


/// <summary>
/// 修改测试表
/// </summary>
[Map(typeof(TestTable))]
public class UpdateTestTableCommand : IRequest<Result<long>>
{

        
        /// <summary>
        /// 名称
        /// </summary>
        [Description("名称")]
        public string Name { get; set; }
        
        /// <summary>
        /// 描述
        /// </summary>
        [Description("描述")]
        public string Description { get; set; }
        
        /// <summary>
        /// 时间
        /// </summary>
        [Description("时间")]
        public DateTime DateTime { get; set; }
        
        /// <summary>
        /// 类型
        /// </summary>
        [Description("类型")]
        public int Type { get; set; }
        
        /// <summary>
        /// 状态
        /// </summary>
        [Description("状态")]
        public bool Stuts { get; set; }
        
        /// <summary>
        /// 唯一标识
        /// </summary>
        [Description("唯一标识")]
        public long TestTableId { get; set; }
        
        /// <summary>
        /// 乐观并发标记
        /// </summary>
        [Description("乐观并发标记")]
        public string ConcurrencyStamp { get; set; }
}

/// <summary>
/// 处理程序
/// </summary>
public class UpdateTestTableCommandHandler : IRequestHandler<UpdateTestTableCommand, Result<long>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UpdateTestTableCommandHandler(
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
    public async Task<Result<long>> Handle(UpdateTestTableCommand request, CancellationToken cancellationToken)
    {
        var testtable = await _context.TestTables
           .SingleOrDefaultAsync(x => x.Id == request.TestTableId, cancellationToken)
           ?? throw new NotFoundException($"数据【{request.TestTableId}】未找到");

        testtable = _mapper.Map(request, testtable);
        //testtable.AddDomainEvent(new UpdatedEvent<TestTable>(testtable));
        _context.TestTables.Update(testtable);
        var isSuccess = await _context.SaveChangesAsync(cancellationToken) > 0;
        return await Result<long>.SuccessOrFailureAsync(testtable.Id, isSuccess, new string[] { "操作失败" });
    }
}
