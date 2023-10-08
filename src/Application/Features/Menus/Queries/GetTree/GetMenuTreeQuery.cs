﻿using Application.Features.Menus.DTOs;
using AutoMapper.QueryableExtensions;
using Domain.Services;

namespace Application.Features.Menus.Queries.GetTree;


public class GetMenuTreeQuery : IRequest<Result<IEnumerable<MenuDto>>>
{
}

public class GetMenuTreeQueryHandler :
    IRequestHandler<GetMenuTreeQuery, Result<IEnumerable<MenuDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly MenuDomainService _menuDomainService;
    private readonly IMapper _mapper;

    public GetMenuTreeQueryHandler(
        IApplicationDbContext context,
        IMapper mapper
,
        MenuDomainService menuDomainService)
    {
        _context = context;
        _mapper = mapper;
        _menuDomainService = menuDomainService;
    }

    public async Task<Result<IEnumerable<MenuDto>>> Handle(GetMenuTreeQuery request, CancellationToken cancellationToken)
    {
        var menus = await _menuDomainService.GetMenuTreeAsync();
        return await Result<IEnumerable<MenuDto>>.SuccessAsync(_mapper.Map<List<MenuDto>>(menus));
    }
}