using Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

/// <summary>
/// ���Խӿ�
/// </summary>
public class WeatherForecastController : ApiControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IApplicationDbContext _context;

    public WeatherForecastController(
        ILogger<WeatherForecastController> logger,
        IApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    /// <summary>
    /// �ӿڵ���
    /// </summary>
    /// <returns></returns>
    [HttpGet(Name = "GetWeatherForecast")]
    [AllowAnonymous]
    public int Get()
    {
        return 0;
    }
}
