namespace Application.Features.Menus.DTOs;

public class UserMenuDto
{
    public List<MenuDto>? Menu { get; set; }
    public List<string>? Permissions { get; set; }
    public List<string>? DashboardGrid { get; set; }
}
