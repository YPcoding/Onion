﻿using Application.Features.TestTables.Caching;
using Domain.Entities;
using Domain.Entities;

namespace Application.Features.TestTables.Commands.Delete;

/// <summary>
/// 删除测试表
/// </summary>
public class DeleteTestTableCommand : IRequest<Result<bool>>
{
  
        /// <summary>
        /// 唯一标识
        /// </summary>
        [Description("唯一标识")]
        public List<long> TestTableIds { get; set; }
}

/// <summary>
/// 处理程序
/// </summary>
public class DeleteTestTableCommandHandler : IRequestHandler<DeleteTestTableCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public DeleteTestTableCommandHandler(
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
    public async Task<Result<bool>> Handle(DeleteTestTableCommand request, CancellationToken cancellationToken)
    {
        var testtablesToDelete = await _context.TestTables
            .Where(x => request.TestTableIds.Contains(x.Id))
            .ToListAsync();

        if (testtablesToDelete.Any())
        {
            _context.TestTables.RemoveRange(testtablesToDelete);
            var isSuccess = await _context.SaveChangesAsync(cancellationToken) > 0;
            return await Result<bool>.SuccessOrFailureAsync(isSuccess, isSuccess,new string[] {"操作失败"});
        }

        return await Result<bool>.FailureAsync(new string[] { "没有找到需要删除的数据" });
    }
}