using Masuit.Tools;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Application.Common.Helper.WebApiDocHelper;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    [AllowAnonymous]
    public IEnumerable<WeatherForecast> Get()
    {
        List<ControllerInfo> controllers = GetWebApiControllersWithActions();

        foreach (var controller in controllers)
        {
            foreach (var item in controller.Actions)
            {
                var key = $"api.{controller.ControllerName}.{item.Route}";
                if (item.Route.IsNullOrEmpty())
                {
                    key = $"api.{controller.ControllerName}";
                }
                key = key.ToLower();
            }
        }

        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now,
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }

  
}
