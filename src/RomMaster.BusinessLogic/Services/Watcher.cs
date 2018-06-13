namespace RomMaster.BusinessLogic.Services
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using RomMaster.Common;

    public class Watcher : IHostedService, IDisposable
    {
        ILogger<Watcher> logger;
        IOptions<AppSettings> appSettings;

        FileSystemWatcher watcher;

        public Watcher(ILogger<Watcher> logger, IOptions<AppSettings> appSettings)
        {
            this.logger = logger;
            this.appSettings = appSettings;
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

        public Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Starting");

            //_timer = new Timer(DoWork, null, TimeSpan.Zero,
            //    TimeSpan.FromSeconds(5));

            var path = appSettings.Value.DatRoots.First().Path;

            watcher = new FileSystemWatcher(path, "*.*");
            watcher.IncludeSubdirectories = true;
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
                                   | NotifyFilters.FileName | NotifyFilters.DirectoryName;

            watcher.Changed += new FileSystemEventHandler(OnChanged);
            //watcher.Created += new FileSystemEventHandler(OnChanged);
            watcher.Deleted += new FileSystemEventHandler(OnChanged);
            watcher.Renamed += new RenamedEventHandler(OnRenamed);
            watcher.Error += new ErrorEventHandler(OnError);

            watcher.EnableRaisingEvents = true;

            return Task.CompletedTask;
        }

        //private void DoWork(object state)
        //{
        //    _logger.LogInformation($"Background work with text: {_appConfig.Value.TextToPrint}");
        //}

        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Stopping.");

            // _timer?.Change(Timeout.Infinite, 0);
            watcher.EnableRaisingEvents = false;

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            // _timer?.Dispose();
            watcher?.Dispose();
        }
    }
}
