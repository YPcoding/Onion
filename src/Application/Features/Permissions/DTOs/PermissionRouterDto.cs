namespace Application.Features.Permissions.DTOs
{
    public class PermissionRouterDto
    {
        public string Path { get; set; }
        public string? Name { get; set; }
        public string? Component { get; set; }
        public Meta Meta { get; set; }
        public PermissionRouterDto[]? Children { get; set; }
    }

    public class Meta 
    {
        public string Title { get; set; }
        public string? Icon { get; set; }
        public int? Rank { get; set; }
        public string[]? Roles { get; set; }
        public string[]? Auths { get; set; }
    }
}
