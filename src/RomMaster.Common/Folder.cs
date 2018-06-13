namespace RomMaster.Common
{
    using System.IO;

    public class Folder
    {
        public string Path { get; set; }

        public SearchOption SearchOptions { get; set; } = SearchOption.TopDirectoryOnly;

        public bool WatcherEnabled { get; set; }
    }
}