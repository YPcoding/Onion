using Application.Common.Extensions;
using Domain.Entities.Identity;
using Application.Features.Menus.Caching;
using Application.Features.Menus.DTOs;
using Application.Features.Menus.Specifications;

namespace Application.Features.Menus.Queries.Pagination;

/// <summary>
/// 菜单管理分页查询
/// </summary>
[Description("分页查询菜单数据")]
public class MenusWithPaginationQuery : MenuAdvancedFilter, IRequest<Result<PaginatedData<MenuDto>>>
{
    [JsonIgnore]
    public MenuAdvancedPaginationSpec Specification => new MenuAdvancedPaginationSpec(this);
}

/// <summary>
/// 处理程序
/// </summary>
public class MenusWithPaginationQueryHandler :
    IRequestHandler<MenusWithPaginationQuery, Result<PaginatedData<MenuDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    public MenusWithPaginationQueryHandler(
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
    /// <returns>返回菜单管理分页数据</returns>
    public async Task<Result<PaginatedData<MenuDto>>> Handle(
        MenusWithPaginationQuery request,
        CancellationToken cancellationToken)
    {
        var menus = await _context.Menus
            .OrderBy($"{request.OrderBy} {request.SortDirection}")
            .ProjectToPaginatedDataAsync<Menu, MenuDto>(
            request.Specification,
            request.PageNumber,
            request.PageSize,
            _mapper.ConfigurationProvider,
            cancellationToken);

        return await Result<PaginatedData<MenuDto>>.SuccessAsync(menus);
    }
}
