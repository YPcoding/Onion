namespace Application.Common.Configurations;

public class SystemSettings
{
    public const string Key = nameof(SystemSettings);
    public string HostDomainName { get; set; }
    public string[] PermissionWhiteListUserNames { get; set; }
}
