using Application.Common;
using Application.Common.Interfaces;
using Common.Quartzs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace WebAPI.Controllers;

/// <summary>
/// 测试接口
/// </summary>
public class WeatherForecastController : ApiControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly IHubContext<SignalRHub> _hubContext;
    private readonly ISnowFlakeService _snowFlakeService;
    private readonly IApplicationDbContext _dbContext;
    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IApplicationDbContext _context;
    private readonly IQuartzService _quartzService;

    public WeatherForecastController(
        ILogger<WeatherForecastController> logger,
        IApplicationDbContext context,
        IHubContext<SignalRHub> hubContext,
        ISnowFlakeService snowFlakeService,
        IApplicationDbContext dbContext,
        IQuartzService quartzService)
    {
        _logger = logger;
        _context = context;
        _hubContext = hubContext;
        _snowFlakeService = snowFlakeService;
        _dbContext = dbContext;
        _quartzService = quartzService;
    }

    /// <summary>
    /// 接口调试
    /// </summary>
    /// <returns></returns>
    [HttpGet(Name = "GetWeatherForecast")]
    [AllowAnonymous]
    public async Task<int> Get()
    {
        await _quartzService.StopAsync();
        return 0;
    }
}
