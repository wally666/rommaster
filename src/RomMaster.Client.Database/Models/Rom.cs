namespace RomMaster.Client.Database.Models
{
    public class Rom
    {
        public int Id { get; private set; }

        public string Name { get; set; }

        public uint Size { get; set; }

        public string Crc { get; set; }

        public string Sha1 { get; set; }

        public string Md5 { get; set; }

        //public string Merge { get; set; }

        //public string Status { get; set; }

        //public string Date { get; set; }
    }
}