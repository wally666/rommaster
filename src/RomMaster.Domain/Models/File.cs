using RomMaster.Common.Database;

namespace RomMaster.Domain.Models
{
    public class File : IEntity
    {
        public int Id { get; private set; }

        public string Path { get; set; }

        public long Size { get; set; }

        public string Crc { get; set; }

        public string Sha1 { get; set; }

        public string Md5 { get; set; }
    }
}