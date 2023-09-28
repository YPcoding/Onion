using System.ComponentModel.DataAnnotations;
using Domain.Entities;
using Application.Features.TestTables.Caching;
using Domain.Entities;
using Masuit.Tools.Systems;
using Microsoft.Extensions.Options;

namespace Application.Features.TestTables.Commands.Add;

/// <summary>
/// 添加测试表
/// </summary>
[Map(typeof(TestTable))]
public class AddTestTableCommand : IRequest<Result<long>>
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
}
/// <summary>
/// 处理程序
/// </summary>
public class AddTestTableCommandHandler : IRequestHandler<AddTestTableCommand, Result<long>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public AddTestTableCommandHandler(
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
    public async Task<Result<long>> Handle(AddTestTableCommand request, CancellationToken cancellationToken)
    {
        var testtable = _mapper.Map<TestTable>(request);
        testtable.AddDomainEvent(new CreatedEvent<TestTable>(testtable));
        await _context.TestTables.AddAsync(testtable);
        var isSuccess = await _context.SaveChangesAsync(cancellationToken) > 0;
        return await Result<long>.SuccessOrFailureAsync(testtable.Id, isSuccess, new string[] { "操作失败" });
    }
}