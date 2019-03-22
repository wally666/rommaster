using Blazor.FlexGrid;
using Microsoft.Extensions.DependencyInjection;
using RomMaster.WebSite.App.Services;
using Microsoft.AspNetCore.Components.Builder;
using RomMaster.Domain.Models;

namespace RomMaster.WebSite.App
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddFlexGrid(
                cfg =>
                {
                    cfg.ApplyConfiguration(new Models.GridConfigurations.DatGridConfiguration());
                    //cfg.ApplyConfiguration(new FileGridConfiguration());
                    //cfg.ApplyConfiguration(new GameGridConfiguration());
                    //cfg.ApplyConfiguration(new RomGridConfiguration());
                },
                options =>
                {
                    options.IsServerSideBlazorApp = true;
                });

            // Since Blazor is running on the server, we can use an application service
            // to read the forecast data.
            services.AddHttpClient("default", configureClient =>
            {
                configureClient.BaseAddress = new System.Uri("https://localhost:60971/");
            });

            services.AddSingleton<LazyDataSet<Dat>>();
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}
