namespace Application.Common.Models;

/// <summary>
/// 分页数据
/// </summary>
/// <typeparam name="T"></typeparam>
public class PaginatedData<T>
{
    /// <summary>
    /// 当前页
    /// </summary>
    public int CurrentPage { get; private set; }

    /// <summary>
    /// 总条数
    /// </summary>
    public int TotalItems { get; private set; }

    /// <summary>
    /// 总页数
    /// </summary>
    public int TotalPages { get; private set; }

    /// <summary>
    /// 有上一页
    /// </summary>
    public bool HasPreviousPage => CurrentPage > 1;

    /// <summary>
    /// 有下一页
    /// </summary>
    public bool HasNextPage => CurrentPage < TotalPages;

    /// <summary>
    /// 返回数据
    /// </summary>
    public IEnumerable<T> Items { get; set; }

    /// <summary>
    /// 分页数据
    /// </summary>
    /// <param name="items">数据</param>
    /// <param name="total">总条数</param>
    /// <param name="pageIndex">页码</param>
    /// <param name="pageSize">每页大小</param>
    public PaginatedData(IEnumerable<T> items, int total, int pageIndex, int pageSize)
    {
        Items = items;
        TotalItems = total;
        CurrentPage = pageIndex;
        TotalPages = (int)Math.Ceiling(total / (double)pageSize);
    }

    /// <summary>
    /// 创建分页
    /// </summary>
    /// <param name="source">数据源</param>
    /// <param name="pageIndex">页码</param>
    /// <param name="pageSize">每页大小</param>
    /// <returns></returns>
    public static async Task<PaginatedData<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
    {
        var count = await source.CountAsync();
        var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PaginatedData<T>(items, count, pageIndex, pageSize);
    }
}
