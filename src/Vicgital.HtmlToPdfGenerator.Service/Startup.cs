using Microsoft.Extensions.Configuration;
using Serilog;
using Vicgital.Core.Configuration.Extensions;
using Vicgital.Core.Configuration.Helpers;
using Vicgital.Core.Logging.Extensions;
using Vicgital.HtmlToPdfGenerator.Business.Components.Definition;
using Vicgital.HtmlToPdfGenerator.Business.Components.Implementation;
using Vicgital.HtmlToPdfGenerator.Service.Services;

namespace Vicgital.HtmlToPdfGenerator.Service;

public class Startup
{
    public static IConfiguration? Configuration { get; set; }

    public void ConfigureServices(IServiceCollection services)
    {

        Configuration = ConfigurationHelpers.GetConfiguration();

        // // Add services to the container.
        services.AddGrpc();
        services.AddAppConfigurationService(Configuration);
        services.AddSerilogConsoleLogging(Configuration);

        // // Add Components 
        services
             .AddSingleton<IPdfGeneratorComponent, PdfGeneratorComponent>();
        

        Log.Information("Service Ready..");

    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGrpcService<HtmlToPdfGeneratorService>();
            endpoints.MapGet("/", async context =>
            {
                await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
            });
        });
    }

}
