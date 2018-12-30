using Microsoft.AspNetCore.Blazor.Builder;
using Microsoft.Extensions.DependencyInjection;
using RomMaster.WebSite.App.Services;
using System.Net.Http;

namespace RomMaster.WebSite.App
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // Since Blazor is running on the server, we can use an application service
            // to read the forecast data.
            services.AddSingleton<WeatherForecastService>();
            services.AddHttpClient("default", configureClient =>
            {
                configureClient.BaseAddress = new System.Uri("https://localhost:60971/");
            });
        }

        public void Configure(IBlazorApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}
