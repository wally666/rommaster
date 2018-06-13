namespace RomMaster.DatFileParser.Models
{
    using System;
    using System.Xml.Serialization;

    /*
    
        <game name="a51mxr3k" cloneof="area51mx" romof="area51mx">
		    <description>Area 51 / Maximum Force Duo (R3000)</description>
		    <year>1998</year>
		    <manufacturer>Atari Games</manufacturer>
		    <disk name="area51mx" merge="area51mx" sha1="5ff10f4e87094d4449eabf3de7549564ca568c7e"/>
	    </game>

    */

    [Serializable()]
    public class Game
    {
        [XmlAttribute("name")]
        public string Name { get; set; } // Air Conflicts - Aces of World War II (USA)

        [XmlAttribute("cloneof")]
        public string CloneOf { get; set; } // area51mx

        [XmlAttribute("romof")]
        public string RomOf { get; set; } // area51mx

        [XmlElement("description")]
        public string Description { get; set; } // Air Conflicts - Aces of World War II (USA)

        [XmlElement("year")]
        public int Year { get; set; } // 1998

        [XmlElement("manufacturer")]
        public string Manufacturer { get; set; } // Atari Games

        [XmlElement("disk")]
        public Disk Disk { get; set; }

        [XmlElement("rom")]
        public Rom[] Roms { get; set; }
    }
}