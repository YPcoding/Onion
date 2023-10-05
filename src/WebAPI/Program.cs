using Application.Common.Interfaces;
using CInfrastructure.Persistence;
using Microsoft.AspNetCore.SignalR;
using System.Text;
using WebAPI;

var builder = WebApplication.CreateBuilder(args);

builder.RegisterSerilog();
builder.Services.AddInfrastructureServices(builder.Configuration)
    .AddApplicationServices()
    .AddWebAPIServices()
    .AddAutoMapper();

var app = builder.Build();

app.UseInfrastructure(builder.Configuration);

//var hubContext = app.Services.GetRequiredService<IHubContext<SignalRHub>>();
//var consoleWriter = new WebApiConsoleWriter(hubContext);
//Console.SetOut(consoleWriter);

if (app.Environment.IsDevelopment())
{
    // Initialise and seed database
    using (var scope = app.Services.CreateScope())
    {
        var initializer = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitializer>();
        await initializer.InitialiseAsync();
        await initializer.SeedAsync();
    }
}
await app.RunAsync();


/// <summary>
/// 重定向控制台使用SignalR输出到前端
/// </summary>
public class WebApiConsoleWriter : TextWriter
{
    private readonly IHubContext<SignalRHub> _hubContext;
    private readonly StringBuilder _buffer = new StringBuilder();
    private readonly TimeSpan _sendInterval = TimeSpan.FromMilliseconds(1); // 调整发送间隔

    public WebApiConsoleWriter(IHubContext<SignalRHub> hubContext)
    {
        _hubContext = hubContext;
        Task.Factory.StartNew(() => PeriodicSendBufferAsync(), TaskCreationOptions.LongRunning);
    }

    public override Encoding Encoding => Encoding.UTF8;

    public override void Write(char value)
    {
        _buffer.Append(value);
    }

    public override void Write(string? value)
    {
        _buffer.Append(value);
    }

    private async Task PeriodicSendBufferAsync()
    {
        while (true)
        {
            await Task.Delay(_sendInterval);

            if (_buffer.Length > 0)
            {
                var message = _buffer.ToString();
                _buffer.Clear();

                await _hubContext.Clients.All.SendAsync("ReceiveNotification", message);
            }
        }
    }
}