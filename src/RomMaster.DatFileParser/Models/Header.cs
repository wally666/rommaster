namespace RomMaster.DatFileParser.Models
{
    using System;
    using System.Xml.Serialization;

    [Serializable()]
    public class Header
    {
        [XmlElement("name")]
        public string Name { get; set; } // fix_Sony - PlayStation Portable
        [XmlElement("description")]
        public string Description { get; set; } // fix_Sony - PlayStation Portable
        [XmlElement("category")]
        public string Category { get; set; } // FIXDATFILE
        [XmlElement("version")]
        public string Version { get; set; } // 03.17.2018 09:51:13
        [XmlElement("date")]
        public string Date { get; set; } // 03.17.2018
        [XmlElement("author")]
        public string Author { get; set; } // RomVault
    }
}