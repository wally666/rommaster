namespace RomMaster.Console
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using BusinessLogic.Services;
    using Client.Database;
    using Common;
    using Common.Database;
    using RomMaster.DatFileParser;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using System.Security.Cryptography;

    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.SetBasePath(Environment.CurrentDirectory)
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                        .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json",
                            optional: true, reloadOnChange: false)
                        .AddEnvironmentVariables();

                    if (args != null)
                    {
                        config.AddCommandLine(args);
                    }
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddOptions()
                        .Configure<AppSettings>(hostContext.Configuration.GetSection("AppSettings"))
                        .AddDbContext<DatabaseContext>(options =>
                        {
                            options.UseSqlite(hostContext.Configuration.GetSection("AppSettings")
                                    .GetConnectionString("sqlite"))
                                .EnableSensitiveDataLogging(false);
                        })
                        .AddSingleton<IUnitOfWorkFactory, UnitOfWorkFactory>()
                        .AddSingleton<Parser>()
                        .AddSingleton<FileWatcherService>()
                        .AddSingleton<DatFileService>()
                        .AddSingleton<RomFileService>()
                        .AddSingleton<ToSortFileService>()
                        .AddSingleton<HashAlgorithm, Force.Crc32.Crc32Algorithm>()
                        .AddSingleton<IHostedService, ClientService>()
                        .Replace(ServiceDescriptor.Singleton(typeof(ILogger<>), typeof(TimedLogger<>)));
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"))
                        .AddConsole();
                });

            await builder.RunConsoleAsync();
        }
    }
}
