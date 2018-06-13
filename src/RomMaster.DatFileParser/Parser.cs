namespace RomMaster.DatFileParser
{
    using System.IO;
    using System.Xml;
    using System.Xml.Serialization;

    public class Parser
    {
        private readonly XmlReaderSettings settings;
        private readonly XmlSerializer serializer;

        public Parser()
        {
            settings = new XmlReaderSettings();
            // settings.DtdProcessing = DtdProcessing.Parse;
            settings.DtdProcessing = DtdProcessing.Ignore;
            settings.MaxCharactersFromEntities = 1024;

            serializer = new XmlSerializer(typeof(Models.DataFile));
        }
        
        public Models.DataFile Parse(string filePathName)
        {
            using (var stream = new FileStream(filePathName, FileMode.Open))
            {
                return Parse(stream);
            }
        }

        public Models.DataFile Parse(Stream stream)
        {
            using (XmlReader reader = XmlReader.Create(stream, settings))
            {
                return (Models.DataFile)serializer.Deserialize(reader);
            }
        }
    }
}
