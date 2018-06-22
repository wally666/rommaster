namespace RomMaster.BusinessLogic.Services
{
    using System.IO;
    using System.Linq;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Common;
    using Common.Database;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;

    public class ClientService : BackgroundService
    {
        private readonly ILogger<ClientService> logger;
        private readonly IOptions<AppSettings> appSettings;
        private readonly IUnitOfWorkFactory unitOfWorkFactory;
        private readonly FileWatcherService fileWatcherService;
        private readonly DatFileService datFileService;

        public ClientService(ILogger<ClientService> logger, IOptions<AppSettings> appSettings, IUnitOfWorkFactory unitOfWorkFactory, FileWatcherService fileWatcherService, DatFileService datFileService)
        {
            this.logger = logger;
            this.appSettings = appSettings;
            this.unitOfWorkFactory = unitOfWorkFactory;
            this.fileWatcherService = fileWatcherService;
            this.datFileService = datFileService;

            this.fileWatcherService.DatFileAdded += DatFileAdded;
        }

        private void DatFileAdded(object sender, FileSystemEventArgs e)
        {
            this.datFileService.Enqueue(e.FullPath);
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await fileWatcherService.StartAsync(cancellationToken);
            await datFileService.StartAsync(cancellationToken);

            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            logger.LogDebug($"{this.GetType()} is starting.");
            cancellationToken.Register(() => logger.LogDebug($"{this.GetType()} background task is stopping."));

            foreach (var datRoot in appSettings.Value.DatRoots)
            {
                logger.LogDebug($"Processing DatRoot folder '{datRoot.Path}' ({datRoot.SearchOptions})");
                if (!Directory.Exists(datRoot.Path))
                {
                    logger.LogWarning($"DatRoot folder '{datRoot.Path}' does not exist. Skipping.");
                    continue;
                }

                var dats = System.IO.Directory.EnumerateFiles(datRoot.Path, "*.*", datRoot.SearchOptions);
                var datCount = dats.Count();
                var index = 0;
                foreach (var dat in dats)
                {
                    logger.LogInformation($"Processing Dat file '{dat}' ({++index}/{datCount})");
                    if (cancellationToken.IsCancellationRequested)
                    {
                        logger.LogWarning($"Processing Dat file '{dat}' ({++index}/{datCount}) has been cancelled.");
                        return;
                    }

                    datFileService.Enqueue(dat);
                }

                logger.LogDebug($"Finished processing DatRoot folder '{datRoot.Path}'. Found {datCount} files.");
            }

            logger.LogDebug($"{this.GetType()} background task is stopping.");
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            // Run background task clean-up actions
            await fileWatcherService.StopAsync(cancellationToken);
            await datFileService.StopAsync(cancellationToken);
        }
    }
}
