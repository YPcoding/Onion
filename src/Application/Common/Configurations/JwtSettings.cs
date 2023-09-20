namespace Application.Common.Configurations;

public class JwtSettings
{
    public const string Key = nameof(JwtSettings);
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public string SecurityKey { get; set; }
    public int ExpireSeconds { get; set; }
    public int RefreshExpiresSeconds { get; set; }
}
