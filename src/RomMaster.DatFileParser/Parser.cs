namespace RomMaster.DatFileParser
{
    using System.IO;
    using System.Xml;
    // using System.Xml.Schema;
    using System.Xml.Serialization;

    public class Parser
    {
        private readonly XmlReaderSettings settings;
        private readonly XmlSerializer serializer;

        public Parser()
        {
            settings = new XmlReaderSettings();
            // settings.DtdProcessing = DtdProcessing.Ignore;
            settings.DtdProcessing = DtdProcessing.Parse;
            settings.ValidationType = ValidationType.None;
            // settings.ValidationType = ValidationType.DTD;
            // settings.ValidationEventHandler += new ValidationEventHandler(ValidationCallBack);

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

        public void Validate(Stream stream)
        {
            var dtdSettings = new XmlReaderSettings();
            // settings.DtdProcessing = DtdProcessing.Ignore;
            dtdSettings.DtdProcessing = DtdProcessing.Parse;
            // dtdSettings.ValidationType = ValidationType.DTD;
            // dtdSettings.ValidationEventHandler += new ValidationEventHandler(ValidationCallBack);

            settings.Schemas.Add(null, XmlReader.Create("datafile.dtd", dtdSettings));

            using (var dtdStream = new FileStream("datafile.dtd", FileMode.Open, FileAccess.Read))
            using (XmlReader reader = XmlReader.Create(stream, settings))
            {
                // XmlSchema schema = XmlSchema.Read(dtdStream, ValidationCallBack);

                //XmlDocument doc = new XmlDocument();

                //doc.Schemas.Add(schema);
                //doc.Schemas.Compile();

                // doc.Load(reader);

                // doc.Validate(ValidationCallBack);

                while (reader.Read());
            }
        }

        //private static void ValidationCallBack(object sender, ValidationEventArgs e)
        //{
        //    System.Console.WriteLine("Validation Error: {0}", e.Message);
        //}
    }
}
