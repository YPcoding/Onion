using WebAPI;

var builder = WebApplication.CreateBuilder(args);

builder.RegisterSerilog();
builder.Services.AddInfrastructureServices(builder.Configuration)
    .AddApplicationServices()
    .AddWebAPIServices()
    .AddAutoMapper();

var app = builder.Build();

app.UseInfrastructure(builder.Configuration);



await app.RunAsync();