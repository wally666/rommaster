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

            while (!stoppingToken.IsCancellationRequested)
            {
                var item = await Task.Run(() => queue.Take(stoppingToken), stoppingToken);
                logger.LogInformation($"{this.GetType()} background task is procesing [{queue.Count}] item '{item}'.");
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
                var repoDat = uow.GetRepository<Dat>();
                DatFileParser.Models.DataFile datFile;

                if (await repoDat.FindAsync(a => a.File != null && a.File.Path == item.File) != null)
                {
                    logger.LogDebug($"DatFile '{item.File}' already processed. Skipping.");
                    return;
                }

                try
                {
                    datFile = this.datFileParser.Parse(item.File);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, ex.Message);
                    return;
                }

                Dat dat = await repoDat.FindAsync(a => a.Name == datFile.Header.Name && a.Version == datFile.Header.Version);
                if (dat != null)
                {
                    logger.LogDebug($"DatFile '{item.File}' duplicated. Skipping.");
                    return;
                }

                dat = new Dat
                {
                    Name = datFile.Header.Name,
                    Description = datFile.Header.Description,
                    Version = datFile.Header.Version,
                    Category = datFile.Header.Category,
                    Author = datFile.Header.Author,
                    Date = ParseDateTime(datFile.Header.Date),
                    File = new File
                    {
                        Path = item.File
                    }
                };

                foreach (var game in datFile.Games)
                {
                    var g = new Game
                    {
                        Name = game.Name,
                        Description = game.Description,
                        Year = game.Year
                    };

                    foreach (var rom in game.Roms)
                    {
                        var r = new Rom
                        {
                            Name = rom.Name,
                            Size = rom.Size,
                            Crc = rom.Crc,
                            Md5 = rom.Md5,
                            Sha1 = rom.Sha1
                        };

                        g.Roms.Add(r);
                    }

                    dat.Games.Add(g);
                }

                await repoDat.AddAsync(dat);
                
                try
                {
                    await uow.CommitAsync();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, ex.Message);
                }
            }
        }

        private DateTime? ParseDateTime(string date)
        {
            if (string.IsNullOrEmpty(date))
            {
                return null;
            }

            DateTime result;

            if (DateTime.TryParseExact(date, "MM-dd-yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out result))
            {
                return result;
            }

            //20140807 16-00-31
            if (DateTime.TryParseExact(date, "yyyyMMdd HH-mm-ss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out result))
            {
                return result;
            }

            return null;
        }
    }
}
