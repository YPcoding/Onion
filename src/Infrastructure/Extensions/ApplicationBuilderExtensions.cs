﻿using IGeekFan.AspNetCore.Knife4jUI;
using Microsoft.Extensions.FileProviders;

namespace Infrastructure.Extensions;
public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app, IConfiguration config)
    {
        app.UseHttpsRedirection();
        app.UseStaticFiles();
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
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapSwagger("{documentName}/api-docs");
        });
        return app;
    }
}
