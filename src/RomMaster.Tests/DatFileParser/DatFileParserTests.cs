namespace RomMaster.Tests.DatFileParser
{
    using FluentAssertions;
    using System.IO;
    using System.Linq;
    using Xunit;

    public class DatFileParserTests
    {
        private readonly RomMaster.DatFileParser.Parser parser;

        public DatFileParserTests()
        {
            parser = new RomMaster.DatFileParser.Parser();
        }

        [Fact]
        public void PassingTest()
        {
            Assert.True(true);
        }

        [Theory]
        [InlineData("fixDat_Sony - PlayStation Portable (20180309-050057).dat")]
        public void Parse_ForValidDatFile_ReturnsParsedData(string filePathName)
        { 
            var result = parser.Parse(filePathName);

            result.Should().NotBeNull();
        }

        [Theory]
        [InlineData("fixDat_Sony - PlayStation Portable (20180309-050057).dat")]
        public void Parse_ForValidDatStream_ReturnsParsedData(string filePathName)
        {
            using (var stream = new FileStream(filePathName, FileMode.Open))
            {
                var result = parser.Parse(stream);

                result.Should().NotBeNull();
            }
        }

        [Theory]
        [InlineData("fixDat_Sony - PlayStation Portable (20180309-050057).dat")]
        public void Validate_ForValidDatStream_DoesNotThrowException(string filePathName)
        {
            using (var stream = new FileStream(filePathName, FileMode.Open))
            {
                parser.Validate(stream);
            }
        }

        [Theory]
        [InlineData("fixDat_Sony - PlayStation Portable (20180309-050057).dat")]
        public void FilterGeneratedReport(string filePathName)
        { 
            var outputFilePathName = @"JDownloader.dat";
            var parser = new RomMaster.DatFileParser.Parser();
            var model = parser.Parse(filePathName);

            filePathName = @"1fichier.com-PSP.txt";
            using (var sr = new StreamReader(filePathName))
            using (var sw = new StreamWriter(outputFilePathName))
            {
                var regex = new System.Text.RegularExpressions.Regex("^(?<GAME_NAME>.*?)\\.7z");
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    var gameName = regex.Match(line).Groups["GAME_NAME"].Value;

                    var game = model.Games.FirstOrDefault(a => a.Name == gameName);
                    if (game != null)
                    {
                        System.Console.WriteLine(line);
                        sw.WriteLine(line);
                    }
                }

                sw.Flush();
                sw.Close();
            }
        }
    }
}
