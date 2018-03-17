namespace RomMaster.DatFileParser.Models
{
    using System;
    using System.Xml.Serialization;

    [Serializable()]
    public class Rom
    {
        [XmlAttribute("name")]
        public string Name { get; set; } // Air Conflicts - Aces of World War II (USA).iso
        [XmlAttribute("size")]
        public uint Size { get; set; } // 900825088
        [XmlAttribute("crc")]
        public string Crc { get; set; } // 1a36c729
        [XmlAttribute("sha1")]
        public string Sha1 { get; set; } // 39a3f9f1bc6b1faa69f4d7b1fd086baab7c5d711
        [XmlAttribute("md5")]
        public string Md5 { get; set; } // 6d4e5e80182e438ded1f10eec6265644
    }
}