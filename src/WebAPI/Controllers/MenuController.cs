using Application.Features.Menus.DTOs;
using Application.Features.Menus.Commands.AddEdit;
using Application.Features.Menus.Commands.Delete;
using Application.Features.Menus.Commands.Update;
using Application.Features.Menus.Queries.Pagination;
using Microsoft.AspNetCore.Mvc;
using Application.Features.Menus.Queries.GetTree;
using System.ComponentModel;

namespace WebAPI.Controllers;

/// <summary>
/// 菜单管理
/// </summary>
[Description("菜单管理")]
public class MenuController : ApiControllerBase
{
    /// <summary>
    /// 分页查询
    /// </summary>
    /// <returns></returns>
    [HttpPost("PaginationQuery")]
    public async Task<Result<PaginatedData<MenuDto>>> PaginationQuery(MenusWithPaginationQuery query)
    {
        return await Mediator.Send(query);
    }

    /// <summary>
    /// 保存菜单管理
    /// </summary>
    /// <returns></returns>
    [HttpPost("Save")]

    public async Task<Result<long>> AddEdit(AddEditMenuCommand command)
    {
        return await Mediator.Send(command);
    }

    /// <summary>
    /// 修改菜单管理
    /// </summary>
    /// <returns></returns>
    [HttpPut("Update")]
    public async Task<Result<long>> Update(UpdateMenuCommand command)
    {
        return await Mediator.Send(command);
    }

    /// <summary>
    /// 删除菜单管理
    /// </summary>
    /// <returns></returns>
    [HttpDelete("Delete")]
    public async Task<Result<bool>> Delete(DeleteMenuCommand command)
    {
        return await Mediator.Send(command);
    }

    /// <summary>
    /// 菜单树
    /// </summary>
    /// <returns></returns>
    [HttpPost("Tree")]
    public async Task<Result<IEnumerable<MenuDto>>> GetMenuTree()
    {
        return await Mediator.Send(new GetMenuTreeQuery());
    }
}
