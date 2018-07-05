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

    public class FixService : BackgroundService
    {
        protected readonly ILogger<FixService> logger;
        private readonly IOptions<AppSettings> appSettings;
        protected readonly IUnitOfWorkFactory unitOfWorkFactory;
        private readonly ManualResetEvent manualResetEvent = new ManualResetEvent(false);

        public FixService(ILogger<FixService> logger, IOptions<AppSettings> appSettings, IUnitOfWorkFactory unitOfWorkFactory)
        {
            this.logger = logger;
            this.appSettings = appSettings;
            this.unitOfWorkFactory = unitOfWorkFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogDebug($"{this.GetType()} is starting.");
            stoppingToken.Register(() => logger.LogDebug($"{this.GetType()} background task is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                await WaitForEventAsync(stoppingToken);
                logger.LogInformation($"{this.GetType()} background task is procesing.");
                await Process();
            }

            logger.LogDebug($"{this.GetType()} background task is stopping.");
        }

        public void SetEvent()
        {
            manualResetEvent.Set();
        }

        private async Task WaitForEventAsync(CancellationToken cancellationToken)
        {
            await Task.Factory.StartNew(() => manualResetEvent.WaitOne(), cancellationToken);
            manualResetEvent.Reset();
        }

        private Task Process()
        {
            using (var uow = this.unitOfWorkFactory.Create())
            {
                var repoFile = uow.GetRepository<File>();

                logger.LogInformation($"Files in DB: {repoFile.GetAll().Count()} background task is stopping.");
            }

            return Task.CompletedTask;
        }
    }
}