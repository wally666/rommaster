namespace RomMaster.BusinessLogic.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    //using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    //using RomMaster.Client.Database;
    //using RomMaster.Client.Database.Models;
    using RomMaster.Common;
    using RomMaster.Common.Database;

    public class Watcher : IHostedService, IDisposable
    {
        private readonly ILogger<Watcher> logger;
        private readonly IOptions<AppSettings> appSettings;
        private readonly IUnitOfWorkFactory unitOfWorkFactory;

        private readonly List<FileSystemWatcher> watchers = new List<FileSystemWatcher>();

        public Watcher(ILogger<Watcher> logger, IOptions<AppSettings> appSettings, IUnitOfWorkFactory unitOfWorkFactory)
        {
            this.logger = logger;
            this.appSettings = appSettings;
            this.unitOfWorkFactory = unitOfWorkFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Starting");

            watchers.AddRange(CreateWatchers(appSettings.Value.DatRoots));
            watchers.AddRange(CreateWatchers(appSettings.Value.RomRoots));
            watchers.AddRange(CreateWatchers(appSettings.Value.ToSortRoots));

            //using (var uow = unitOfWorkFactory.Create())
            //{
            //    var repo = uow.GetRepository<Dat>();
            //    var any = repo.All().Any();
            //}

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Stopping.");

            watchers.ForEach((watcher) => watcher.EnableRaisingEvents = false);

            return Task.CompletedTask;
        }

        private IEnumerable<FileSystemWatcher> CreateWatchers(List<Folder> datRoots)
        {
            foreach (var path in appSettings.Value.DatRoots)
            {
                var watcher = new FileSystemWatcher(path.Path, "*.*");
                watcher.IncludeSubdirectories = path.WatcherEnabled;
                watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
                                       | NotifyFilters.FileName | NotifyFilters.DirectoryName;

                watcher.Changed += new FileSystemEventHandler(OnChanged);
                watcher.Created += new FileSystemEventHandler(OnChanged); // relevant? duplicated?
                watcher.Deleted += new FileSystemEventHandler(OnChanged);
                watcher.Renamed += new RenamedEventHandler(OnRenamed);
                watcher.Error += new ErrorEventHandler(OnError);

                watcher.EnableRaisingEvents = path.WatcherEnabled;

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
