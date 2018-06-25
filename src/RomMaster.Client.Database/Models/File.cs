namespace RomMaster.Client.Database.Models
{
    public class File
    {
        public int Id { get; private set; }

        public string Path { get; set; }

        public uint Size { get; set; }

        public string Crc { get; set; }

        public string Sha1 { get; set; }

        public string Md5 { get; set; }
    }
}