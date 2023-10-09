using Application.Common.Extensions;
using Domain.Entities.Identity;
using Application.Features.Menus.Caching;
using Application.Features.Menus.DTOs;
using Application.Features.Menus.Specifications;
using AutoMapper.QueryableExtensions;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.Menus.Queries.GetById;

/// <summary>
/// 通过唯一标识获取一条数据
/// </summary>
[Description("通过唯一标识查询单条菜单数据")]
public class GetMenuQueryById : IRequest<Result<MenuDto>>
{
    /// <summary>
    /// 唯一标识
    /// </summary>
    [Required(ErrorMessage = "唯一标识必填的")]
    public long MenuId { get; set; }
}

/// <summary>
/// 处理程序
/// </summary>
public class GetMenuByIdQueryHandler :IRequestHandler<GetMenuQueryById, Result<MenuDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetMenuByIdQueryHandler(
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
    public async Task<Result<MenuDto>> Handle(GetMenuQueryById request, CancellationToken cancellationToken)
    {
        var menu = await _context.Menus.ApplySpecification(new MenuByIdSpec(request.MenuId))
                     .ProjectTo<MenuDto>(_mapper.ConfigurationProvider)
                     .SingleOrDefaultAsync(cancellationToken) ?? throw new NotFoundException($"唯一标识: [{request.MenuId}] 未找到");
        return await Result<MenuDto>.SuccessAsync(menu);
    }
}
