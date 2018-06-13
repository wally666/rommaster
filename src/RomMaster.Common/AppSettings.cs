namespace RomMaster.Common
{
    using System.Collections.Generic;

    public class AppSettings
    {
        public List<Folder> DatRoots { get; set; } = new List<Folder>();

        public List<Folder> RomRoots { get; set; } = new List<Folder>();

        public List<Folder> ToSortRoots { get; set; } = new List<Folder>();
    }
}
