namespace RomMaster.BusinessLogic.Services
{
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Common;
    using Common.Database;
    using System.Security.Cryptography;
    using System.Collections.Generic;
    using RomMaster.BusinessLogic.Models;
    using System.Threading.Tasks;
    using RomMaster.Client.Database.Models;

    public class RomFileService : FileService
    {
        //int have = 0;

        public RomFileService(ILogger<RomFileService> logger, IOptions<AppSettings> appSettings, IUnitOfWorkFactory unitOfWorkFactory, HashAlgorithm crc32)
            : base(logger, appSettings, unitOfWorkFactory, crc32)
        {
        }

        protected override IEnumerable<Folder> GetFolders(IOptions<AppSettings> appSettings)
        {
            return appSettings.Value.RomRoots;
        }

        //protected override Task<List<File>> Process(FileQueueItem item)
        //{
        //    return base.Process(item);
        //}

        //protected override async Task PostProcess(File file)
        //{
        //    // find dat
        //    Rom rom;
        //    using (var uow = unitOfWorkFactory.Create())
        //    {
        //        var repoRom = uow.GetRepository<Rom>();
        //        rom = await repoRom.FindAsync(a => a.Crc == file.Crc && a.Crc != null || a.Size == file.Size && (a.Crc == null || file.Crc == null));
        //    }

        //    if (rom != null)
        //    {
        //        if (IsArchive(file.Path))
        //        {
        //            return;
        //        }

        //        // ensure size
        //        System.Diagnostics.Debug.Assert(/*file.Size == 0 || */rom.Size == file.Size, "File and Rom have equal Crc but the sizes are different.");

        //        // ensure crc
        //        System.Diagnostics.Debug.Assert(file.Crc == rom.Crc, "File and Rom have equal Crc but the sizes are different.");

        //        have++;

        //        this.logger.LogInformation($"Found '{file.Path}' as '{rom.Name}' [{have}]");
        //    }

        //    // set as Have
        //    // ...
        //}
    }
}