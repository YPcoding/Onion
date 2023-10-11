using Domain.Entities.Loggers;

namespace Application.Features.Loggers.DTOs;

[Map(typeof(Logger))]
public class LoggerDto
{     

    /// <summary>
    /// 
    /// </summary>
    public long LoggerId 
    {
        get 
        {
            return Id;
        }
    }    

    /// <summary>
    /// 
    /// </summary>
    [Description("")]
    public long Id { get; set; }    

    /// <summary>
    /// 
    /// </summary>
    [Description("")]
    public string Timestamp { get; set; }    

    /// <summary>
    /// 
    /// </summary>
    [Description("")]
    public string Level { get; set; }    

    /// <summary>
    /// 
    /// </summary>
    [Description("")]
    public string Template { get; set; }    

    /// <summary>
    /// 
    /// </summary>
    [Description("")]
    public string Message { get; set; }    

    /// <summary>
    /// 
    /// </summary>
    [Description("")]
    public string Exception { get; set; }    

    /// <summary>
    /// 
    /// </summary>
    [Description("")]
    public string Properties { get; set; }    

    /// <summary>
    /// 
    /// </summary>
    [Description("")]
    public DateTime? TS { get; set; }
}

public class CountDailyDto 
{
    public List<string> XAxis { get; set; } = new List<string>();
    public List<int> InformationConut { get; set; } = new List<int>();
    public List<int> WarningConut { get; set; } = new List<int>();
    public List<int> ErrorConut { get; set; } = new List<int>();

}