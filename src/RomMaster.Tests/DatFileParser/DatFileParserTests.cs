namespace RomMaster.Tests.DatFileParser
{
    using FluentAssertions;
    using Xunit;

    public class DatFileParserTests
    {
        private readonly RomMaster.DatFileParser.Parser parser;

        public DatFileParserTests()
        {
            parser = new RomMaster.DatFileParser.Parser();
        }

        [Theory]
        [InlineData("fixDat_Sony - PlayStation Portable (20180309-050057).dat")]
        public void Parse_ForValidDatFile_ReturnsParsedData(string filePathName)
        { 
            var result = parser.Parse(filePathName);

            result.Should().NotBeNull();
        }
    }
}
