namespace RomMaster.BusinessLogic.Services
{
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Models;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Common;
    using Common.Database;
    using System.Security.Cryptography;
    using System;
    using RomMaster.Client.Database.Models;
    using System.Collections.Generic;

    public abstract class FileService : BackgroundService
    {
        protected readonly ILogger<FileService> logger;
        private readonly IOptions<AppSettings> appSettings;
        protected readonly IUnitOfWorkFactory unitOfWorkFactory;
        private readonly HashAlgorithm crc32;
        private readonly BlockingCollection<FileQueueItem> queue = new BlockingCollection<FileQueueItem>();
        private readonly ManualResetEvent queueIsEmpty = new ManualResetEvent(false);

        List<Exclude> excludes;
        protected List<Exclude> Excludes
        {
            get
            {
                if (excludes == null)
                {
                    excludes = GetFolders(this.appSettings).SelectMany(a => a.Excludes).ToList();
                }

                return excludes;
            }
        }

        public FileService(ILogger<FileService> logger, IOptions<AppSettings> appSettings, IUnitOfWorkFactory unitOfWorkFactory, HashAlgorithm crc32)
        {
            this.logger = logger;
            this.appSettings = appSettings;
            this.unitOfWorkFactory = unitOfWorkFactory;
            this.crc32 = crc32;
        }

        protected abstract IEnumerable<Folder> GetFolders(IOptions<AppSettings> appSettings);

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            foreach (var folder in GetFolders(this.appSettings))
            {
                logger.LogDebug($"Processing folder '{folder.Path}' ({folder.SearchOptions})");
                if (!System.IO.Directory.Exists(folder.Path))
                {
                    logger.LogWarning($"Folder '{folder.Path}' does not exist. Skipping.");
                    continue;
                }

                var files = System.IO.Directory.EnumerateFiles(folder.Path, "*.*", folder.SearchOptions);
                var filesCount = files.Count();
                var index = 0;
                foreach (var file in files)
                {
                    ++index;
                    logger.LogInformation($"Enqueuing [{(float)index / filesCount * 100,3:000}] file '{file}' ({index}/{filesCount})");
                    if (cancellationToken.IsCancellationRequested)
                    {
                        logger.LogWarning($"Processing file '{file}' ({index}/{filesCount}) has been cancelled.");
                        return;
                    }

                    Enqueue(file);
                }

                logger.LogDebug($"Finished processing folder '{folder.Path}'. Found {filesCount} files.");
            }

            await base.StartAsync(cancellationToken);
        }

        private bool IsExcluded(string file)
        {
            return IsExcluded(file, Excludes);
        }
        
        private bool IsExcluded(string file, List<Exclude> excludes)
        {
            if (!excludes.Any())
            {
                return false;
            }

            foreach (var exclude in excludes)
            {
                if (IsExcluded(file, exclude))
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsExcluded(string file, Exclude exclude)
        {
            return exclude.Match(file);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogDebug($"{this.GetType()} is starting.");
            stoppingToken.Register(() => logger.LogDebug($"{this.GetType()} background task is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                if (!queue.Any())
                {
                    queueIsEmpty.Set();
                }

                var item = await Task.Run(() => queue.Take(stoppingToken), stoppingToken);
                logger.LogInformation($"{this.GetType()} background task is procesing [{queue.Count}] item '{item}'.");
                var files = await Process(item);
                foreach (var file in files)
                {
                    await PostProcess(file);
                }
            }

            logger.LogDebug($"{this.GetType()} background task is stopping.");
        }

        public void Enqueue(string file)
        {
            var item = new FileQueueItem
            {
                File = file
            };

            if (IsExcluded(file))
            {
                logger.LogInformation($"File processing '{file}' excluded. Skipped.");
                return;
            }

            queue.Add(item);
            queueIsEmpty.Reset();
        }

        public async Task WaitForQueueEmptyAsync(CancellationToken cancellationToken)
        {
            await Task.Factory.StartNew(() => queueIsEmpty.WaitOne(), cancellationToken);
        }

        protected virtual async Task<List<File>> Process(FileQueueItem item)
        {
            File file = null;
            List<File> files = new List<File>();
            using (var uow = unitOfWorkFactory.Create())
            {
                var repoFile = uow.GetRepository<File>();
                if (await repoFile.AnyAsync(a => a.Path == item.File))
                {
                    logger.LogDebug($"File '{item.File}' already processed. Skipped.");
                    return files;
                }

                //archive
                if (IsArchive(item.File))
                {
                    try
                    {
                        using (System.IO.Stream stream = System.IO.File.OpenRead(item.File))
                        using (var archive = SharpCompress.Archives.ArchiveFactory.Open(stream))
                        {
                            foreach (var entry in archive.Entries.Where(a => !a.IsDirectory))
                            {
                                var fileName = $"{item.File}#{entry.Key}";

                                if (await repoFile.AnyAsync(a => a.Path == fileName))
                                {
                                    continue;
                                }

                                // store file info
                                file = new File
                                {
                                    Crc = entry.Crc.ToString("X2"),
                                    Path = fileName,
                                    Size = entry.Size
                                };

                                await repoFile.AddAsync(file);
                                files.Add(file);
                            }
                        }
                    }
                    catch (SharpCompress.Common.ArchiveException ex)
                    {
                        logger.LogError(ex, $"File '{item.File}' corrupted.");
                        return files;
                    }
                    catch (InvalidOperationException ex)
                    {
                        logger.LogError(ex, $"File '{item.File}' error.");
                        return files;
                    }
                }

                // add file regardless it is archive
                string computedCrc32 = null;
                long size = 0;
                if (!IsArchive(item.File))
                {
                    using (var stream = System.IO.File.Open(item.File, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read))
                    {
                        size = stream.Length;
                        if (await repoFile.AnyAsync(a => a.Size == size))
                        {
                            var hash = crc32.ComputeHash(stream);
                            computedCrc32 = BitConverter.ToString(hash).Replace("-", "");
                        }
                    }
                }

                // store file info
                file = new File
                {
                    Crc = computedCrc32, //null if archive
                    Path = item.File,
                    Size = size //0 if archive
                };

                await repoFile.AddAsync(file);
                files.Add(file);

                await uow.CommitAsync();
            }

            return files;
        }

        protected virtual Task PostProcess(File file)
        {
            return Task.CompletedTask;
        }

        protected bool IsArchive(string file)
        {
            switch (System.IO.Path.GetExtension(file).ToLower())
            {
                //Zip, GZip, BZip2, Tar, Rar, LZip, XZ'
                case ".rar":
                case ".zip":
                    return true;
                case ".7z":
                    return true;
                default:
                    return false;
            }
        }
    }
}