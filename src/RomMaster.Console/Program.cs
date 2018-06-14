namespace RomMaster.Console
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using RomMaster.BusinessLogic.Services;
    using RomMaster.Common;

    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: true);
                    config.AddEnvironmentVariables();

                    if (args != null)
                    {
                        config.AddCommandLine(args);
                    }
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddOptions();
                    services.Configure<AppSettings>(hostContext.Configuration.GetSection("AppSettings"));

                    services.AddSingleton<IHostedService, Watcher>();
                })
                .ConfigureLogging((hostingContext, logging) => {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                });

            await builder.RunConsoleAsync();
        }

        //const string datFileRootFolder = @"z:\RomVault\DatRoot";

        //static void Main(string[] args)
        //{
        //    RomMaster.FileSystemWatcher.Watcher w = new RomMaster.FileSystemWatcher.Watcher(@"z:\RomVault\ToSort");
        //    w.Enabled = true;

        //    RomMaster.FileSystemWatcher.Watcher watcherDatRoot = new RomMaster.FileSystemWatcher.Watcher(datFileRootFolder);
        //    watcherDatRoot.Enabled = true;

        //    Console.ReadLine();

        //    var outputFilePathName = @"c:\repo\github_wally666\rommaster\src\RomMaster.Tests\JDownloader.dat";
        //    var filePathName = @"c:\repo\github_wally666\rommaster\src\RomMaster.Tests\fixDat_Sony - PlayStation Portable (20180309-050057).dat";
        //    var parser = new RomMaster.DatFileParser.Parser();
        //    var model = parser.Parse(filePathName);

        //    filePathName = @"c:\repo\github_wally666\rommaster\src\RomMaster.Tests\1fichier.com-PSP.txt";
        //    using (var sr = new StreamReader(filePathName))
        //    using (var sw = new StreamWriter(outputFilePathName))
        //    {
        //        var regex = new System.Text.RegularExpressions.Regex("^(?<GAME_NAME>.*?)\\.7z");
        //        while (!sr.EndOfStream)
        //        {
        //            var line = sr.ReadLine();
        //            var gameName = regex.Match(line).Groups["GAME_NAME"].Value;

        //            var game = model.Games.FirstOrDefault(a => a.Name == gameName);
        //            if (game != null)
        //            {
        //                System.Console.WriteLine(line);
        //                sw.WriteLine(line);
        //            }
        //        }

        //        sw.Flush();
        //        sw.Close();
        //    }
        //}
    }
}
