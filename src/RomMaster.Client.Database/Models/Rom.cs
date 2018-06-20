namespace RomMaster.Client.Database.Models
{
    public class Rom
    {
        public int Id { get; private set; }

        public string Name { get; set; }

        public int Size { get; set; }

        public string Crc { get; set; }

        public string Sha1 { get; set; }

        public string Md5 { get; set; }
    }
}