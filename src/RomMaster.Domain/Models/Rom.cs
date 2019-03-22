using RomMaster.Common.Database;

namespace RomMaster.Domain.Models
{
    public class Rom : IEntity
    {
        public int Id { get; private set; }

        public string Name { get; set; }

        public long Size { get; set; }

        public string Crc { get; set; }

        public string Sha1 { get; set; }

        public string Md5 { get; set; }

        //public string Merge { get; set; }

        //public string Status { get; set; }

        //public string Date { get; set; }
    }
}