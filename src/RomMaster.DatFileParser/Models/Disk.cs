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
    public class Disk
    {
        //[XmlAttribute("name")]
        //public string Name { get; set; } // area51mx

        [XmlAttribute("merge")]
        public string Merge { get; set; } // area51mx

        [XmlAttribute("name")]
        public string Sha1 { get; set; } // 5ff10f4e87094d4449eabf3de7549564ca568c7e
    }
}