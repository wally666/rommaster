namespace RomMaster.BusinessLogic.Services
{
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Common;
    using Common.Database;
    using System.Security.Cryptography;
    using System.Collections.Generic;
    using RomMaster.Client.Database.Models;
    using System.Threading.Tasks;

    public class ToSortFileService : FileService
    {
        public ToSortFileService(ILogger<ToSortFileService> logger, IOptions<AppSettings> appSettings, IUnitOfWorkFactory unitOfWorkFactory, HashAlgorithm crc32)
            : base(logger, appSettings, unitOfWorkFactory, crc32)
        {
        }

        protected override IEnumerable<Folder> GetFolders(IOptions<AppSettings> appSettings)
        {
            return appSettings.Value.ToSortRoots;
        }

        //protected override async Task PostProcess(File file)
        //{
        //    // find dat
        //    Rom rom;
        //    using (var uow = unitOfWorkFactory.Create())
        //    {
        //        var repoRom = uow.GetRepository<Rom>();
        //        rom = await repoRom.FindAsync(a => a.Crc == file.Crc);
        //    }

        //    if (rom != null)
        //    {
        //        // ensure size
        //        System.Diagnostics.Debug.Assert(file.Size == 0 || rom.Size == file.Size, "File and Rom have equal Crc but the sizes are different.");

        //        this.logger.LogInformation($"Found '{file.Path}' as '{rom.Name}'");

        //        // move to Rom folder
        //        // ...
        //    }
        //}
    }
}