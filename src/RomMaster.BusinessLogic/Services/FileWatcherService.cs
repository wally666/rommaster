namespace RomMaster.BusinessLogic.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Common;
    using System.Linq;

    public class FileWatcherService : IHostedService, IDisposable
    {
        private readonly ILogger<FileWatcherService> logger;
        private readonly IOptions<AppSettings> appSettings;
        private readonly List<FileSystemWatcher> watchers = new List<FileSystemWatcher>();

        public FileSystemEventHandler DatFileChanged { get; set; }
        public FileSystemEventHandler RomFileChanged { get; set; }
        public FileSystemEventHandler ToSortFileChanged { get; set; }

        public FileWatcherService(ILogger<FileWatcherService> logger, IOptions<AppSettings> appSettings)
        {
            this.logger = logger;
            this.appSettings = appSettings;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Starting...");

            watchers.AddRange(CreateWatchers(appSettings.Value.DatRoots, OnDatFileChanged));
            watchers.AddRange(CreateWatchers(appSettings.Value.RomRoots, OnRomFileChanged));
            watchers.AddRange(CreateWatchers(appSettings.Value.ToSortRoots, OnToSortFileChanged));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Stopping...");

            watchers.ForEach((watcher) => watcher.EnableRaisingEvents = false);

            return Task.CompletedTask;
        }

        protected virtual void OnDatFileChanged(object sender, FileSystemEventArgs e)
        {
            DatFileChanged?.Invoke(sender, e);
        }

        protected virtual void OnRomFileChanged(object sender, FileSystemEventArgs e)
        {
            RomFileChanged?.Invoke(sender, e);
        }

        protected virtual void OnToSortFileChanged(object sender, FileSystemEventArgs e)
        {
            ToSortFileChanged?.Invoke(sender, e);
        }

        private IEnumerable<FileSystemWatcher> CreateWatchers(List<Folder> folders, FileSystemEventHandler onFileChanged = null)
        {
            foreach (var folder in folders)
            {
                if (!Directory.Exists(folder.Path))
                {
                    logger.LogWarning($"Folder '{folder.Path}' does not exist. Skipping.");
                    continue;
                }

                var watcher = new FileSystemWatcher(folder.Path, "*.*")
                {
                    IncludeSubdirectories = folder.WatcherEnabled,
                    NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
                                       | NotifyFilters.FileName | NotifyFilters.DirectoryName
                };

                if (onFileChanged != null)
                {
                    watcher.Renamed += (sender, args) =>
                    {
                        OnChanged(onFileChanged, sender, args.ChangeType, args.FullPath, args.Name, folder);
                    };
                    watcher.Created += (sender, args) =>
                    {
                        OnChanged(onFileChanged, sender, args.ChangeType, args.FullPath, args.Name, folder);
                    };
                    watcher.Changed += (sender, args) =>
                    {
                        OnChanged(onFileChanged, sender, args.ChangeType, args.FullPath, args.Name, folder);
                    };
                    watcher.Deleted += (sender, args) =>
                    {
                        OnChanged(onFileChanged, sender, args.ChangeType, args.FullPath, args.Name, folder);
                    };
                }

                watcher.EnableRaisingEvents = folder.WatcherEnabled;

                yield return watcher;
            }
        }

        private void OnChanged(FileSystemEventHandler onChanged, object sender, WatcherChangeTypes changeType, string filePathName, string fileName, Folder folder)
        {
            if (IsExcluded(filePathName, folder.Excludes))
            {
                logger.LogDebug($"File '{filePathName}' excluded from watching.");
            }
            else
            {
                logger.LogDebug($"File '{filePathName}' changed: '{changeType}'.");
                onChanged(sender, new FileSystemEventArgs(changeType, filePathName, fileName));
            }
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

        public void Dispose()
        {
            foreach (var toDispose in watchers.ToArray())
            {
                toDispose?.Dispose();
            }
        }
    }
}
