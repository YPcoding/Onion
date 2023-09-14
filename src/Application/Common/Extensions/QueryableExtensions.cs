using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using Domain.Common;

namespace Application.Common.Extensions;

public static class QueryableExtensions
{
    /// <summary>
    /// ���淶�ṩ��ѯ����չ������
    /// </summary>
    /// <remarks>
    /// �˷���ʹ��SpecificationEvaluator���ݸ����Ĺ淶�������޸��ṩ�Ĳ�ѯ��
    /// </remarks>
    /// <typeparam name="T">��ѯ��ʵ�������</typeparam>
    /// <param name="query">ӦӦ�ù淶��ԭʼ��ѯ</param>
    /// <param name="spec">ҪӦ���ڲ�ѯ�Ĺ淶</param>
    /// <param name="evaluateCriteriaOnly">��ѡ��־������ȷ���Ƿ�ֻӦ������׼����������</param>
    /// <returns>��Ӧ�ù淶���޸Ĳ�ѯ</returns>
    public static IQueryable<T> ApplySpecification<T>(this IQueryable<T> query, ISpecification<T> spec, bool evaluateCriteriaOnly=false) where T : class, IEntity
    {
        return SpecificationEvaluator.Default.GetQuery(query, spec, evaluateCriteriaOnly);
    }
    /// <summary>
    /// Ϊ��ҳ������ṩ����ɲ�ѯ���ݵ���չ������
    /// </summary>
    /// <remarks>
    /// �÷����������Ĺ淶Ӧ���ڲ�ѯ���Խ�����з�ҳ����������ͶӰ������Ľ�����͡�
    /// </remarks>
    /// <typeparam name="T">��ѯ��ʵ���Դ����</typeparam>
    /// <typeparam name="TResult">ʵ��ӦͶӰ����Ŀ������</typeparam>
    /// <param name="query">ҪͶӰ�ͷ�ҳ��ԭʼ�����ѯ</param>
    /// <param name="spec">��ͶӰ�ͷ�ҳ֮ǰӦ���ڲ�ѯ�Ĺ淶</param>
    /// <param name="pageNumber">��ҳ���������ҳ��</param>
    /// <param name="pageSize">��ҳ�����ÿҳ����Ŀ��</param>
    /// <param name="configuration">ͶӰ������</param>
    /// <param name="cancellationToken">����ȡ�������Ŀ�ѡȡ������</param>
    /// <returns>��ҳ��ͶӰ������</returns>
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
