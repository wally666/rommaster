namespace RomMaster.BusinessLogic.Services
{
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Common;
    using Common.Database;
    using System.Security.Cryptography;
    using System.Collections.Generic;

    public class RomFileService : FileService
    {
        public RomFileService(ILogger<RomFileService> logger, IOptions<AppSettings> appSettings, IUnitOfWorkFactory unitOfWorkFactory, HashAlgorithm crc32)
            : base(logger, appSettings, unitOfWorkFactory, crc32)
        {
        }

        protected override IEnumerable<Folder> GetFolders(IOptions<AppSettings> appSettings)
        {
            return appSettings.Value.RomRoots;
        }
    }
}