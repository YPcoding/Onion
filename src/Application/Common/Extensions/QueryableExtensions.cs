using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using Domain.Common;

namespace Application.Common.Extensions;

public static class QueryableExtensions
{
    /// <summary>
    /// 按规范提供查询的扩展方法。
    /// </summary>
    /// <remarks>
    /// 此方法使用SpecificationEvaluator根据给定的规范评估和修改提供的查询。
    /// </remarks>
    /// <typeparam name="T">查询中实体的类型</typeparam>
    /// <param name="query">应应用规范的原始查询</param>
    /// <param name="spec">要应用于查询的规范</param>
    /// <param name="evaluateCriteriaOnly">可选标志，用于确定是否只应评估标准或其他配置</param>
    /// <returns>已应用规范的修改查询</returns>
    public static IQueryable<T> ApplySpecification<T>(this IQueryable<T> query, ISpecification<T> spec, bool evaluateCriteriaOnly=false) where T : class, IEntity
    {
        return SpecificationEvaluator.Default.GetQuery(query, spec, evaluateCriteriaOnly);
    }
    /// <summary>
    /// 为分页结果集提供有序可查询数据的扩展方法。
    /// </summary>
    /// <remarks>
    /// 该方法将给定的规范应用于查询，对结果进行分页，并将它们投影到所需的结果类型。
    /// </remarks>
    /// <typeparam name="T">查询中实体的源类型</typeparam>
    /// <typeparam name="TResult">实体应投影到的目标类型</typeparam>
    /// <param name="query">要投影和分页的原始有序查询</param>
    /// <param name="spec">在投影和分页之前应用于查询的规范</param>
    /// <param name="pageNumber">分页结果的所需页码</param>
    /// <param name="pageSize">分页结果中每页的项目数</param>
    /// <param name="configuration">投影的配置</param>
    /// <param name="cancellationToken">用于取消操作的可选取消令牌</param>
    /// <returns>分页和投影的数据</returns>
    public static async Task<PaginatedData<TResult>> ProjectToPaginatedDataAsync<T,TResult>(this IOrderedQueryable<T> query, ISpecification<T> spec,int pageNumber,int pageSize, IConfigurationProvider configuration, CancellationToken cancellationToken = default) where T : class, IEntity
    {
       
        var specificationEvaluator = SpecificationEvaluator.Default;
        var count =await specificationEvaluator.GetQuery(query,spec).CountAsync();
        var data = await specificationEvaluator.GetQuery(query.AsNoTracking(), spec).Skip((pageNumber-1)* pageSize).Take(pageSize)
            .ProjectTo<TResult>(configuration)
            .ToListAsync(cancellationToken);
        return new PaginatedData<TResult>(data, count, pageNumber, pageSize);
    }
}
