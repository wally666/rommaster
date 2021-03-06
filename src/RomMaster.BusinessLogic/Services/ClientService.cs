﻿namespace RomMaster.BusinessLogic.Services
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
        private readonly FixService fixService;

        public ClientService(ILogger<ClientService> logger, IOptions<AppSettings> appSettings, FileWatcherService fileWatcherService, DatFileService datFileService, RomFileService romFileService, ToSortFileService toSortFileService, FixService fixService)
        {
            this.logger = logger;
            this.appSettings = appSettings;
            this.fileWatcherService = fileWatcherService;
            this.datFileService = datFileService;
            this.romFileService = romFileService;
            this.toSortFileService = toSortFileService;
            this.fixService = fixService;

            this.fileWatcherService.DatFileChanged += DatFileChanged;
            this.fileWatcherService.ToSortFileChanged += ToSortFileChanged;
        }

        private void DatFileChanged(object sender, FileSystemEventArgs e)
        {
            this.datFileService.Enqueue(e.FullPath);
            // TODO: execute FixService for new Dat file data
            // this.fixService.Enqueue(e.FullPath);
        }

        private void ToSortFileChanged(object sender, FileSystemEventArgs e)
        {
            // TODO: wait for access to file
            this.toSortFileService.Enqueue(e.FullPath);
            // TODO: wait for end of thr toSortService processing
            // TODO: execute FixService for new Dat file data
            // this.fixService.Enqueue(e.FullPath);
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogDebug("Statring the application...");

            await fileWatcherService.StartAsync(cancellationToken).ConfigureAwait(false);
            await datFileService.StartAsync(cancellationToken).ConfigureAwait(false);
            await datFileService.WaitForQueueEmptyAsync(cancellationToken).ConfigureAwait(false);

            await romFileService.StartAsync(cancellationToken).ConfigureAwait(false);
            await toSortFileService.StartAsync(cancellationToken).ConfigureAwait(false);

            logger.LogDebug("Application has been started.");
            await base.StartAsync(cancellationToken).ConfigureAwait(false);
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            logger.LogDebug("Starting...");

            cancellationToken.Register(() => logger.LogDebug("Background task is stopping..."));

            await datFileService.WaitForQueueEmptyAsync(cancellationToken).ConfigureAwait(false);
            await romFileService.WaitForQueueEmptyAsync(cancellationToken).ConfigureAwait(false);
            await toSortFileService.WaitForQueueEmptyAsync(cancellationToken).ConfigureAwait(false);

            await fixService.StartAsync(cancellationToken).ConfigureAwait(false);

            logger.LogDebug("Background task is stopping.");
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogDebug("Stopping the application...");

            // Run background task clean-up actions
            await fileWatcherService.StopAsync(cancellationToken).ConfigureAwait(false);
            await datFileService.StopAsync(cancellationToken).ConfigureAwait(false);
            await romFileService.StopAsync(cancellationToken).ConfigureAwait(false);
            await toSortFileService.StopAsync(cancellationToken).ConfigureAwait(false);
            await fixService.StopAsync(cancellationToken).ConfigureAwait(false);

            logger.LogDebug("Application has been stopped.");
        }
    }
}
