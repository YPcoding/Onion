using System.ComponentModel.DataAnnotations;

namespace Application.Common.Configurations;

public class DatabaseSettings : IValidatableObject
{
    /// <summary>
    /// 数据库键约束
    /// </summary>
    public const string Key = nameof(DatabaseSettings);

    /// <summary>
    /// 表示要连接到的数据库提供程序
    /// </summary>
    public string DBProvider { get; set; } = string.Empty;

    /// <summary>
    /// 用于连接给定数据库提供程序的连接字符串
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// 验证输入的配置
    /// </summary>
    /// <param name="validationContext">描述执行验证检查的上下文。</param>
    /// <returns>验证的结果</returns>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrEmpty(DBProvider))
        {
            yield return new ValidationResult(
                $"{nameof(DatabaseSettings)}.{nameof(DBProvider)} is not configured",
                new[] { nameof(DBProvider) });
        }

        if (string.IsNullOrEmpty(ConnectionString))
        {
            yield return new ValidationResult(
                $"{nameof(DatabaseSettings)}.{nameof(ConnectionString)} is not configured",
                new[] { nameof(ConnectionString) });
        }
    }
}
