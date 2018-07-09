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
            await Process(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                var item = await Task.Run(() => queue.Take(stoppingToken), stoppingToken);
                logger.LogInformation($"Background task is procesing [{queue.Count}] item '{item}'.");
                await Process(item);
            }

            logger.LogDebug("Background task is stopping.");
        }

        public void Enqueue(string file)
        {
            var item = new FileQueueItem
            {
                File = file
            };

            queue.Add(item);
        }

        private Task Process(CancellationToken stoppingToken)
        {
            logger.LogInformation("Finding fixes...");

            using (var uow = this.unitOfWorkFactory.Create())
            {
                var repoFile = uow.GetRepository<File>();
                var files = repoFile.SqlQuery($@"SELECT f.*
FROM File f
JOIN Rom r ON f.crc = r.crc AND f.size = r.size
WHERE 
f.size <> 0 
ORDER BY f.path");

//                var repoRom = uow.GetRepository<Rom>();
//                var roms = repoRom.SqlQuery($@"SELECT r.*
//FROM Dat d
//JOIN Game g ON d.id = g.datid
//JOIN Rom r ON r.gameid = g.id
//JOIN File f ON f.size <> 0 AND f.crc = r.crc AND f.size = r.size");

                // Enqueue()
                var foundCount = files.Count();
                logger.LogInformation($"Found '{foundCount}' files to fix.");
            }

            return Task.CompletedTask;
        }

        private async Task Process(FileQueueItem item)
        {
            logger.LogInformation($"Finding fix for '{item.File}'...");

            using (var uow = this.unitOfWorkFactory.Create())
            {
                var repoFile = uow.GetRepository<File>();
                var file = await repoFile.FindAsync(a => a.Path == item.File);
                if (file == null)
                {
                    return;
                }

                logger.LogDebug($"Found file '{file.Path}'");
            }
        }
    }
}