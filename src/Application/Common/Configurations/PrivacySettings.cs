namespace Application.Common.Configurations;

/// <summary>
/// 应用程序设置
/// </summary>
public class PrivacySettings
{
    /// <summary>
    /// 获取用于标识PrivacySettings配置的唯一密钥。
    /// </summary>
    public const string Key = nameof(PrivacySettings);

    /// <summary>
    /// 获取或设置一个值，该值指示是否应记录客户端IP地址。
    /// </summary>
    public bool LogClientIpAddresses { get; set; } = true;

    /// <summary>
    /// 获取或设置一个值，该值指示是否应记录客户端代理（用户代理）。
    /// </summary>
    public bool LogClientAgents { get; set; } = true;

    /// <summary>
    /// 获取或设置一个值，该值指示是否应使用Google Analytics进行跟踪。
    /// </summary>
    public bool UseGoogleAnalytics { get; set; }

    /// <summary>
    /// 获取或设置Google Analytics跟踪密钥。
    /// </summary>
    /// <remarks>
    /// 如果 <see cref="UseGoogleAnalytics" /> 设置为true，则此属性必须包含有效的Google Analytics跟踪密钥。
    /// </remarks>
    public string? GoogleAnalyticsKey { get; set; }
}
