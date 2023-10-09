using Application.Features.Menus.DTOs;
using Domain.Services;

namespace Application.Features.Menus.Queries.GetByUserId;

[Description("查询用户菜单数据")]
public class GetMenusQueryByUserId : IRequest<Result<UserMenuDto>>
{
    public long UserId { get; set; }
}

public class GetMenusQueryByUserIdHandler :
    IRequestHandler<GetMenusQueryByUserId, Result<UserMenuDto>>
{
    private readonly IMapper _mapper;
    private readonly MenuDomainService _domainService;

    public GetMenusQueryByUserIdHandler(
        IMapper mapper,
        MenuDomainService domainService)
    {
        _mapper = mapper;
        _domainService = domainService;
    }

    public async Task<Result<UserMenuDto>> Handle(GetMenusQueryByUserId request, CancellationToken cancellationToken)
    {
        var menus = await _domainService.GetTreeByUserIdAsync(request.UserId);

        var permissions = (await _domainService.GetPermissionsAsync(request.UserId))
            .Select(s => s.Code)
            .ToList();

        var dashboardGrid = await _domainService.GetDashboardGridsAsync();

        return await Result<UserMenuDto>.SuccessAsync(new UserMenuDto
        {
            Menu = _mapper.Map<List<MenuDto>>(menus),
            Permissions = permissions!,
            DashboardGrid = dashboardGrid
        });
    }
}
