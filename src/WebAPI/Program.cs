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

var hubContext = app.Services.GetRequiredService<IHubContext<SignalRHub>>();
var consoleWriter = new WebApiConsoleWriter(hubContext);
Console.SetOut(consoleWriter);

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
/// ��ȡ����̨�����Ϣ�����ض���
/// </summary>
public class WebApiConsoleWriter : TextWriter
{
    private readonly IHubContext<SignalRHub> _hubContext;

    public WebApiConsoleWriter(IHubContext<SignalRHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public override Encoding Encoding => Encoding.UTF8;

    public override async void Write(char value)
    {
        // ���ַ����͵� SignalR Hub ���������ʵ���λ��
        await _hubContext.Clients.All.SendAsync("ReceiveNotification", value.ToString());
    }

    public override async void Write(string? value)
    {
        // ���ַ������͵� SignalR Hub ���������ʵ���λ��
        await _hubContext.Clients.All.SendAsync("ReceiveNotification", value);
    }
}
