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

    public class FileWatcherService : IHostedService, IDisposable
    {
        private readonly ILogger<FileWatcherService> logger;
        private readonly IOptions<AppSettings> appSettings;
        private readonly List<FileSystemWatcher> watchers = new List<FileSystemWatcher>();

        public FileSystemEventHandler DatFileAdded { get; set; }
        public FileSystemEventHandler ToSortFileAdded { get; set; }

        public FileWatcherService(ILogger<FileWatcherService> logger, IOptions<AppSettings> appSettings)
        {
            this.logger = logger;
            this.appSettings = appSettings;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Starting");

            watchers.AddRange(CreateWatchers(appSettings.Value.DatRoots, OnDatFileAdded));
            watchers.AddRange(CreateWatchers(appSettings.Value.RomRoots));
            watchers.AddRange(CreateWatchers(appSettings.Value.ToSortRoots, OnToSOrtFileAdded));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Stopping.");

            watchers.ForEach((watcher) => watcher.EnableRaisingEvents = false);

            return Task.CompletedTask;
        }

        protected virtual void OnDatFileAdded(object sender, FileSystemEventArgs e)
        {
            DatFileAdded?.Invoke(sender, e);
        }

        protected virtual void OnToSOrtFileAdded(object sender, FileSystemEventArgs e)
        {
            ToSortFileAdded?.Invoke(sender, e);
        }

        private IEnumerable<FileSystemWatcher> CreateWatchers(List<Folder> folders, FileSystemEventHandler onChanged = null)
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

                watcher.Changed += OnChanged;
                watcher.Created += OnChanged; // relevant? duplicated?
                watcher.Deleted += OnChanged;
                watcher.Renamed += OnRenamed;
                watcher.Error += OnError;

                if (onChanged != null)
                {
                    watcher.Renamed += (sender, args) =>
                    {
                        onChanged(sender, new FileSystemEventArgs(args.ChangeType, args.FullPath, args.Name));
                    };
                    watcher.Created += onChanged;
                    watcher.Changed += onChanged;
                }

                watcher.EnableRaisingEvents = folder.WatcherEnabled;

                yield return watcher;
            }
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            Console.WriteLine($"FileSystemWatcher.OnError: {e.GetException()}");
        }

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            Console.WriteLine($"FileSystemWatcher.OnRenamed: {e.ChangeType}, {e.FullPath}, {e.Name}, {e.OldFullPath}, {e.OldName}");
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"FileSystemWatcher.OnChanged: {e.ChangeType}, {e.FullPath}, {e.Name}");
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
