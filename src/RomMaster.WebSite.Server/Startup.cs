using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using RomMaster.BusinessLogic.Services;
using RomMaster.Client.Database;
using RomMaster.Common;
using RomMaster.Common.Database;
using RomMaster.DatFileParser;
using System.Net.Mime;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;
using Newtonsoft.Json.Serialization;

namespace RomMaster.WebSite.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvc()
                .AddNewtonsoftJson(options => {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            });

            services.AddResponseCompression(options => {
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] {
                    MediaTypeNames.Application.Octet,
                    // WasmMediaTypeNames.Application.Wasm
                });
            });

            services.AddOptions()
                .Configure<AppSettings>(Configuration.GetSection("AppSettings"))
                .AddDbContext<DatabaseContext>(options =>
                {
                    options.UseSqlite(Configuration.GetSection("AppSettings")
                            .GetConnectionString("sqlite"))
                        .EnableSensitiveDataLogging(false);
                }, ServiceLifetime.Singleton)

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
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseResponseCompression();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBlazorDebugging();
            }

            app.UseMvc(routes =>
            {
                routes.MapRoute(name: "default", template: "{controller}/{action}/{id?}");
            });

            app.UseBlazor<App.Startup>();
        }
    }
}
