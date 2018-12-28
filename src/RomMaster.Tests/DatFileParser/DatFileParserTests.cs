namespace RomMaster.Tests.DatFileParser
{
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
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
                // parser.Validate(stream);
            }
        }

        /// <summary>
        /// var a = $("table a");
        /// $.map(a, function(b) { return console.log(b.text + ' - ' + b.href); })
        /// 
        /// ^.*?\d([ ]-[ ])
        /// </summary>
        /// <param name="filePathName"></param>
        [Theory]
        [InlineData("fixDat_Sony - PlayStation Portable (20180309-050057).dat")]
        public void FilterGeneratedReport(string filePathName)
        { 
            var outputFilePathName = @"JDownloader.dat";
            var model = parser.Parse(filePathName);

            filePathName = @"2fichier.com-PSP.txt";
            using (var sr = new StreamReader(filePathName))
            using (var sw = new StreamWriter(outputFilePathName))
            {
                var availableGames = new List<(string Name, string Line)>();
                var regex = new System.Text.RegularExpressions.Regex("^(?<GAME_NAME>.*?)(\\s+\\[.\\])?\\.(7z|zip)");
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    var gameName = regex.Match(line).Groups["GAME_NAME"].Value;

                    availableGames.Add((Name: gameName, Line: line));
                }

                availableGames = availableGames.OrderBy(a => a.Name).ToList();

                foreach (var game in model.Games.OrderBy(a => a.Name)) //.FirstOrDefault(a => missedGame.Name.StartsWith(a.Name) || gameName.StartsWith(a.Name.Replace("'", "_-_")));
                {
                    var missedGame = game.Name; //.Replace("'", "_-_");
                    foreach (var availableGame in availableGames.Where(a => missedGame.StartsWith(a.Name)))
                    {
                        if (availableGame.Name != null)
                        {
                            System.Console.WriteLine(availableGame.Line);
                            sw.WriteLine(availableGame.Line);
                        }
                        else
                        {
                            sw.WriteLine($"? {missedGame}");
                        }
                    }
                }

                sw.Flush();
                sw.Close();
            }
        }
    }
}
