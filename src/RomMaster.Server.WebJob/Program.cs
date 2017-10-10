namespace RomMaster.Server.WebJob
{
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.ServiceBus;
    using System.Collections.Generic;
    using System.IO;
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection;
    using NLog.Extensions.Logging;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Options;
    using Microsoft.Azure.WebJobs.Extensions.SendGrid;
    
    // To learn more about Microsoft Azure WebJobs SDK, please see https://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        private readonly static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public static IConfigurationRoot Configuration { get; set; }

        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        static void Main()
        {
            // Set up Configuration
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            Configuration = builder.Build();

            //
            //
            //

            //Set up DI
            IContainer container = BuildContainer();
            ConfigureServices(container);

            //
            //
            //

            var secrets = container.Resolve<AppSecrets>();

            //

            var config = new JobHostConfiguration();

            if (config.IsDevelopment)
            {
                config.UseDevelopmentSettings();
            }
            
            config.UseTimers();
            config.UseSendGrid(container.Resolve<IOptions<AppOptions>>().Value.SendGrid);

            config.DashboardConnectionString = secrets.WebJobStorageConnectionString;
            config.StorageConnectionString = secrets.WebJobStorageConnectionString;

            config.JobActivator = new AutofacJobActivator(container);

            var host = new JobHost(config);
            // The following code ensures that the WebJob will be running continuously
            host.RunAndBlock();
        }

        /// <summary>
        /// Constructs an application container 
        /// </summary>
        /// <returns>An application container populated with desired types and services</returns>
        static IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();

            //Register local types to be resolved
            builder
                .Register<IAzureSettings>((ctx) => ctx.Resolve<IOptions<AppOptions>>().Value);
            builder
                .RegisterType<Functions>();
            builder
                .RegisterType<KeyVaultService>();
            builder
                .RegisterType<AppSecrets>()
                .AsSelf();
            //...

            //Register logging services for resolution
            var services = new ServiceCollection();
            services.AddLogging();

            // Adds services required for using options.
            services.AddOptions();

            // Register the ConfigurationBuilder instance which MyOptions binds against.
            services.Configure<AppOptions>(options => Configuration.Bind(options));

            builder.Populate(services);

            return builder.Build();
        }

        /// <summary>
        /// Perform any desired configuration on container services
        /// </summary>
        /// <param name="container">The application container to be configured</param>
        static void ConfigureServices(IContainer container)
        {
            using (var scope = container.BeginLifetimeScope())
            {
                //Add NLog as a log consumer
                var loggerFactory = scope.Resolve<Microsoft.Extensions.Logging.ILoggerFactory>();
                loggerFactory.AddNLog(); //notice: the project's only line of code referencing NLog (aside from .config)
            }
        }
    }
}
