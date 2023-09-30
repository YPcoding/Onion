namespace Application.Common.Configurations;

public class SnowFlakeSettings
{
    public const string Key = nameof(SnowFlakeSettings);
    public long WorkerId { get; set; }
    public long DataCenterId { get; set; }
}