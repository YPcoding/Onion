namespace Application.Features.Permissions.DTOs
{
    public class PermissionRouterDto
    {
        public long Id { get; set; }
        public string Path { get; set; }
        public string? Name { get; set; }
        public string? Component { get; set; }
        public Meta Meta { get; set; }
        public List<PermissionRouterDto>? Children { get; set; }
    }

    public class Meta 
    {
        public string Title { get; set; }
        public string? Icon { get; set; }
        public int? Rank { get; set; }
        public string[]? Roles { get; set; }
        public string[]? Auths { get; set; }
    }


    public class MuenDto 
    {
        public List<PermissionMuenDto> Menu { get; set; } = new List<PermissionMuenDto>();
        public List<string> Permissions { get; set; } = new List<string>();
        public List<string> DashboardGrid { get; set; } = new List<string>();
    }

    public class PermissionMuenDto
    {
        public long Id { get; set; }
        public string Path { get; set; }
        public string? Name { get; set; }
        public string? Component { get; set; }
        public MuenMeta Meta { get; set; } = new MuenMeta();
        public List<PermissionMuenDto>? Children { get; set; }
    }

    public class MuenMeta
    {
        public string Title { get; set; }
        public string? Icon { get; set; }
        public string? Type { get; set; }
        public int? Rank { get; set; }
        public string[]? Roles { get; set; }
        public string[]? Auths { get; set; }
        public bool? Fullpage { get; set; }
        public bool? Hidden { get; set; }
        public bool? Affix { get; set;}
        public string? Tag { get; set; }
        public string? Active { get; set; }
    }
}
