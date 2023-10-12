﻿using Microsoft.Extensions.Hosting;

namespace Infrastructure.Services;

public class QuartzStatusBackgroundService : BackgroundService
{
    private readonly QuartzStatusUpdaterAndStarterService _quartzStatusService;

    public QuartzStatusBackgroundService(QuartzStatusUpdaterAndStarterService quartzStatusService)
    {
        _quartzStatusService = quartzStatusService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await _quartzStatusService.UpdateTaskStatusInDatabase();
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken); // 10秒更新一次
        }
    }
}
