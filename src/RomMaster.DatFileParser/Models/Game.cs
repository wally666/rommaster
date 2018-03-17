namespace RomMaster.DatFileParser.Models
{
    using System;
    using System.Xml.Serialization;

    [Serializable()]
    public class Game
    {
        [XmlAttribute("name")]
        public string Name { get; set; } // Air Conflicts - Aces of World War II (USA)
        [XmlElement("description")]
        public string Description { get; set; } // Air Conflicts - Aces of World War II (USA)
        [XmlElement("rom")]
        public Rom[] Roms { get; set; }
    }
}