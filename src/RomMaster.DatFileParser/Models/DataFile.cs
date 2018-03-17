namespace RomMaster.DatFileParser.Models
{
    using System;
    using System.Xml.Serialization;

    [Serializable()]
    [XmlRoot(ElementName = "datafile")]
    public class DataFile
    {
        [XmlElement("header")]
        public Header Header { get; set; }
        [XmlElement("game")]
        public Game[] Games { get; set; }
    }
}
