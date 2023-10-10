using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Loggers;

public class PgLogger : IEntity<int>
{
    public int Id { get; set; }
    public string? Message { get; set; }
    public string? Template { get; set; }
    public string Level { get; set; } = default!;

    public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
    public string? Exception { get; set; }
    //public string? UserName { get; set; }
    //public string? ClientIP { get; set; }
    //public string? ClientAgent { get; set; }
    public string? Properties { get; set; }
    //public string? LogEvent { get; set; }
}
