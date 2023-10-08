using Application.Features.Menus.DTOs;
using Domain.Entities;
using Domain.Services;
using System.Linq;

namespace Application.Features.Menus.Queries.GetByUserId;

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
        var menus = await _domainService
            .GetMenuTreeAsync(x =>
            x.RoleMenus.Any(rp => rp.Role.UserRoles.Any(ur => ur.UserId == request.UserId)) &&
            x.Meta.Type != MetaType.Api &&
            x.Meta.Hidden != true);

        var permissions = (await _domainService.GetPermissionsAsync(x =>
             x.RoleMenus.Any(rp => rp.Role.UserRoles.Any(ur => ur.UserId == request.UserId)) &&
             menus.Select(s => s.Id).ToList().Contains((long)x.ParentId!))).Select(s => s.Code)
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
