namespace RomMaster.BusinessLogic.Services
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Models;
    using Client.Database.Models;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Common;
    using Common.Database;
    using RomMaster.DatFileParser;

    public class DatFileService : BackgroundService
    {
        private readonly ILogger<DatFileService> logger;
        private readonly IOptions<AppSettings> appSettings;
        private readonly IUnitOfWorkFactory unitOfWorkFactory;
        private readonly BlockingCollection<DatFileQueueItem> queue = new BlockingCollection<DatFileQueueItem>();
        private readonly Parser datFileParser;

        public DatFileService(ILogger<DatFileService> logger, IOptions<AppSettings> appSettings, IUnitOfWorkFactory unitOfWorkFactory, Parser datFileParser)
        {
            this.logger = logger;
            this.appSettings = appSettings;
            this.unitOfWorkFactory = unitOfWorkFactory;
            this.datFileParser = datFileParser;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogDebug($"{this.GetType()} is starting.");

            stoppingToken.Register(() => logger.LogDebug($"{this.GetType()} background task is stopping."));

            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken); // TODO

            while (!stoppingToken.IsCancellationRequested)
            {
                var item = queue.Take(stoppingToken); //TODO: replace with async/awaitable call

                logger.LogDebug($"{this.GetType()} background task is procesing item '{item}'.");

                await Process(item);
            }

            logger.LogDebug($"{this.GetType()} background task is stopping.");
        }

        public void Enqueue(string datFilePathName)
        {
            var item = new DatFileQueueItem
            {
                File = datFilePathName
            };

            queue.Add(item);
        }

        private async Task Process(DatFileQueueItem item)
        {
            using (var uow = unitOfWorkFactory.Create())
            {
                var repo = uow.GetRepository<Dat>();
                var datFile = this.datFileParser.Parse(item.File);

                await repo.AddAsync(new Dat
                {
                    Name = datFile.Header.Name,
                    Description = datFile.Header.Description,
                    Version = datFile.Header.Version,
                    Category = datFile.Header.Category,
                    Author = datFile.Header.Author,
                    // Date = datFile.Header.Date
                });

                await uow.CommitAsync();
            }
        }
    }
}
