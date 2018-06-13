namespace RomMaster.Tests.DatFileParser
{
    using FluentAssertions;
    using System.IO;
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
    }
}
