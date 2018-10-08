using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RomMaster.BusinessLogic.Services;
using RomMaster.Client.Database;
using RomMaster.Common;
using RomMaster.Common.Database;
using RomMaster.DatFileParser;
using System.Security.Cryptography;

namespace RomMaster.WebSite
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

            // console
            services.AddOptions()
                .Configure<AppSettings>(Configuration.GetSection("AppSettings"))
                .AddDbContext<DatabaseContext>(options =>
                {
                    options.UseSqlite(Configuration.GetSection("AppSettings")
                            .GetConnectionString("sqlite"))
                        .EnableSensitiveDataLogging(false);
                }, ServiceLifetime.Transient)
                .AddTransient<IUnitOfWorkFactory, UnitOfWorkFactory>()
                .AddSingleton<Parser>()
                .AddSingleton<FileWatcherService>()
                .AddSingleton<DatFileService>()
                .AddSingleton<RomFileService>()
                .AddSingleton<ToSortFileService>()
                .AddSingleton<FixService>()
                .AddSingleton<HashAlgorithm, Force.Crc32.Crc32Algorithm>()
                .AddSingleton<IHostedService, ClientService>()
                .Replace(ServiceDescriptor.Singleton(typeof(ILogger<>), typeof(TimedLogger<>)));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
    }
}
