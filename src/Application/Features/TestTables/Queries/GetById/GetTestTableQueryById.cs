using Application.Common.Extensions;
using Domain.Entities;
using Application.Features.TestTables.Caching;
using Application.Features.TestTables.DTOs;
using Application.Features.TestTables.Specifications;
using AutoMapper.QueryableExtensions;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.TestTables.Queries.GetById;

/// <summary>
/// 通过唯一标识获取一条数据
/// </summary>
public class GetTestTableQueryById : IRequest<Result<TestTableDto>>
{
    /// <summary>
    /// 唯一标识
    /// </summary>
    [Required(ErrorMessage = "唯一标识必填的")]
    public long TestTableId { get; set; }
}

/// <summary>
/// 处理程序
/// </summary>
public class GetTestTableByIdQueryHandler :IRequestHandler<GetTestTableQueryById, Result<TestTableDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetTestTableByIdQueryHandler(
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
    public async Task<Result<TestTableDto>> Handle(GetTestTableQueryById request, CancellationToken cancellationToken)
    {
        var testtable = await _context.TestTables.ApplySpecification(new TestTableByIdSpec(request.TestTableId))
                     .ProjectTo<TestTableDto>(_mapper.ConfigurationProvider)
                     .SingleOrDefaultAsync(cancellationToken) ?? throw new NotFoundException($"唯一标识: [{request.TestTableId}] 未找到");
        return await Result<TestTableDto>.SuccessAsync(testtable);
    }
}
