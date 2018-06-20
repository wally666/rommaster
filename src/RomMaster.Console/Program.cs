namespace RomMaster.Console
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using RomMaster.BusinessLogic.Services;
    using RomMaster.Client.Database;
    using RomMaster.Common;
    using RomMaster.Common.Database;

    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.SetBasePath(Environment.CurrentDirectory);
                    config.AddJsonFile("appsettings.json", optional: false);
                    config.AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", optional: true);
                    config.AddEnvironmentVariables();

                    if (args != null)
                    {
                        config.AddCommandLine(args);
                    }
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddOptions();
                    services.Configure<AppSettings>(hostContext.Configuration.GetSection("AppSettings"));

                    services.AddDbContext<DatabaseContext>(options => options.UseSqlite(hostContext.Configuration.GetSection("AppSettings").GetConnectionString("sqlite")));

                    services.AddSingleton<IUnitOfWorkFactory, UnitOfWorkFactory>();
                    services.AddSingleton<IHostedService, Watcher>();
                })
                .ConfigureLogging((hostingContext, logging) => {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                });

            await builder.RunConsoleAsync();
        }
    }
}
