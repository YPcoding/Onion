using Application.Common;
using IGeekFan.AspNetCore.Knife4jUI;
using Infrastructure.Services;
using Microsoft.Extensions.FileProviders;
using Serilog;

namespace Infrastructure.Extensions;
public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app, IConfiguration config)
    {
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        // 启用响应压缩
        app.UseResponseCompression();
        app.UseExceptionHandler("/Error");
        if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), @"Files")))
        {
            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), @"Files"));
        }
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Files")),
            RequestPath = new PathString("/Files")
        });
        app.UseMiddlewares();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseSwagger();
        app.UseKnife4UI(c =>
        {
            c.RoutePrefix = ""; // serve the UI at root
            c.SwaggerEndpoint("/v1/api-docs", "V1 Docs");
        });
        app.UseCors(builder =>
        {
            builder.SetIsOriginAllowed(origin => true)
                  .AllowAnyHeader()
           .AllowAnyMethod()
           .AllowCredentials();
        });
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers()
            .RequireAuthorization();
            endpoints.MapSwagger("{documentName}/api-docs");
            endpoints.MapHub<SignalRHub>("/signalRHub");
        });
        return app;
    }
}
