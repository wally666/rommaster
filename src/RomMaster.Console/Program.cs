using System;
using System.IO;
using System.Linq;

namespace RomMaster.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var outputFilePathName = @"c:\repo\github_wally666\rommaster\src\RomMaster.Tests\JDownloader.dat";
            var filePathName = @"c:\repo\github_wally666\rommaster\src\RomMaster.Tests\fixDat_Sony - PlayStation Portable (20180309-050057).dat";
            var parser = new RomMaster.DatFileParser.Parser();
            var model = parser.Parse(filePathName);

            filePathName = @"c:\repo\github_wally666\rommaster\src\RomMaster.Tests\1fichier.com-PSP.txt";
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
