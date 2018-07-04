namespace RomMaster.BusinessLogic.Services
{
    using System.IO;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Common;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;

    public class ClientService : BackgroundService
    {
        private readonly ILogger<ClientService> logger;
        private readonly IOptions<AppSettings> appSettings;
        private readonly FileWatcherService fileWatcherService;
        private readonly DatFileService datFileService;
        private readonly RomFileService romFileService;
        private readonly ToSortFileService toSortFileService;

        public ClientService(ILogger<ClientService> logger, IOptions<AppSettings> appSettings, FileWatcherService fileWatcherService, DatFileService datFileService, RomFileService romFileService, ToSortFileService toSortFileService)
        {
            this.logger = logger;
            this.appSettings = appSettings;
            this.fileWatcherService = fileWatcherService;
            this.datFileService = datFileService;
            this.romFileService = romFileService;
            this.toSortFileService = toSortFileService;

            this.fileWatcherService.DatFileAdded += DatFileAdded;
        }

        private void DatFileAdded(object sender, FileSystemEventArgs e)
        {
            this.datFileService.Enqueue(e.FullPath);
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogDebug("Statring the application...");

            await fileWatcherService.StartAsync(cancellationToken);
            await datFileService.StartAsync(cancellationToken);
            await datFileService.WaitForQueueEmptyAsync(cancellationToken);

            await romFileService.StartAsync(cancellationToken);
            await toSortFileService.StartAsync(cancellationToken);

            await base.StartAsync(cancellationToken);

            logger.LogDebug("Application has been started.");
        }

        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            logger.LogDebug($"{this.GetType()} is starting...");

            cancellationToken.Register(() => logger.LogDebug($"{this.GetType()} background task is stopping..."));

            //...

            logger.LogDebug($"{this.GetType()} background task is stopping.");
            return Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogDebug("Stopping the application...");

            // Run background task clean-up actions
            await fileWatcherService.StopAsync(cancellationToken);
            await datFileService.StopAsync(cancellationToken);
            await romFileService.StopAsync(cancellationToken);
            await toSortFileService.StopAsync(cancellationToken);

            logger.LogDebug("Application has been stopped.");
        }
    }
}
