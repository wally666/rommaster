namespace RomMaster.BusinessLogic.Services
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Common;
    using Common.Database;
    using RomMaster.Client.Database.Models;
    using System.Collections.Concurrent;
    using RomMaster.BusinessLogic.Models;

    public class FixService : BackgroundService
    {
        private readonly ILogger<FixService> logger;
        private readonly IOptions<AppSettings> appSettings;
        private readonly IUnitOfWorkFactory unitOfWorkFactory;
        private readonly BlockingCollection<FileQueueItem> queue = new BlockingCollection<FileQueueItem>();

        public FixService(ILogger<FixService> logger, IOptions<AppSettings> appSettings, IUnitOfWorkFactory unitOfWorkFactory)
        {
            this.logger = logger;
            this.appSettings = appSettings;
            this.unitOfWorkFactory = unitOfWorkFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogDebug("Starting...");
            stoppingToken.Register(() => logger.LogDebug("Background task is stopping."));

            logger.LogInformation("Background task is procesing.");
            await Process(stoppingToken).ConfigureAwait(false);

            while (!stoppingToken.IsCancellationRequested)
            {
                var item = await Task.Run(() => queue.Take(stoppingToken), stoppingToken).ConfigureAwait(false);
                logger.LogInformation($"Background task is procesing [{queue.Count}] item '{item}'.");
                await Process(item).ConfigureAwait(false);
            }

            logger.LogDebug("Background task is stopping.");
        }

        public void Enqueue(File file)
        {
            var item = new FileQueueItem
            {
                File = file.Path
            };

            queue.Add(item);
        }

        private Task Process(CancellationToken stoppingToken)
        {
            logger.LogInformation("Finding fixes...");

            using (var uow = this.unitOfWorkFactory.Create())
            {
                var repoFile = uow.GetRepository<File>();
                var files = repoFile.SqlQuery($@"
                    SELECT f.*
                    FROM File f
                    JOIN Rom r ON f.crc = r.crc AND f.size = r.size
                    WHERE f.size <> 0 
                    ORDER BY f.path");

                var foundCount = files.Count();
                logger.LogInformation($"Found '{foundCount}' files to fix (including already fixed).");

                foreach (var file in files)
                {
                    Enqueue(file);
                }
            }

            return Task.CompletedTask;
        }

        private /*async*/ Task Process(FileQueueItem item)
        {
            var proggress = queue.Count;

            if (!System.IO.File.Exists(item.File))
            {
                logger.LogWarning($"[{proggress}] File '{item.File}' does not exist. Skipping.");
                return Task.FromResult<object>(null);
            }

            logger.LogInformation($"[{proggress}] Finding fix for '{item.File}'...");

            //using (var uow = this.unitOfWorkFactory.Create())
            //{
            //    //TODO: Extend FileQueueItem and store the File entity instead querying again
            //    var repoFile = uow.GetRepository<File>();
            //    var file = await repoFile.FindAsync(a => a.Path == item.File).ConfigureAwait(false);

            //    var romRepo = uow.GetRepository<Rom>();
            //    var rom = await romRepo.FindAsync(a => file.Crc == a.Crc).ConfigureAwait(false);
            //    if (rom != null)
            //    {
            //        System.Diagnostics.Debug.Assert(file.Size == rom.Size);
            //        logger.LogInformation($"[{proggress}] Found file '{item.File}' as '{rom.Name}' rom.");
            //    }
            //    else
            //    {
            //        logger.LogInformation($"[{proggress}] Unknown file '{item.File}'.");
            //    }
            //}

            return Task.FromResult<object>(null);
        }
    }
}