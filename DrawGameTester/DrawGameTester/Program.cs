using Autofac;
using InterfurCreations.AdventureGames.Configuration;
using InterfurCreations.AdventureGames.Core;
using InterfurCreations.AdventureGames.Core.Interface;
using InterfurCreations.AdventureGames.GameLanguage;
using InterfurCreations.AdventureGames.Graph;
using InterfurCreations.AdventureGames.Services;
using InterfurCreations.AdventureGames.Services.Interfaces;
using InterfurCreations.AdventureGames.SlackReporter;
using InterfurCreations.AdventureGames.Tester.DrawIOTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawGameTester
{
    class Program
    {
        private static IContainer Container { get; set; }

        public static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<TextParsing>().As<ITextParsing>().InstancePerLifetimeScope();
            builder.RegisterType<GameDataService>().As<IGameDataService>().InstancePerLifetimeScope();
            builder.RegisterType<GameProcessor>().As<IGameProcessor>().InstancePerLifetimeScope();
            builder.RegisterType<DrawStore>().As<IGameStore>().SingleInstance();
            builder.RegisterType<ConsoleAppGoogleDriveAuthenticator>().As<IGoogleDriveAuthenticator>().SingleInstance();
            builder.RegisterType<GoogleDriveService>().As<IGoogleDriveService>().SingleInstance();
            builder.RegisterType<SlackReport>().As<IReporter>().SingleInstance();
            builder.RegisterType<ConfigurationService>().As<IConfigurationService>().SingleInstance();
            builder.RegisterType<DrawGameTestExecutor>().As<DrawGameTestExecutor>().SingleInstance();

            Container = builder.Build();

            while (true)
            {

                using (var scope = Container.BeginLifetimeScope())
                {
                    var drawStore = scope.Resolve<IGameStore>();
                    var testExecutor = scope.Resolve<DrawGameTestExecutor>();
                    var reporter = scope.Resolve<IReporter>();

                    var games = drawStore.ListGames();

                    int i = 0;
                    foreach (var game in games)
                    {
                        Console.Write(i++ + " - ");
                        Console.WriteLine(game.GameName);
                    }

                    Console.Write("Enter game to test: ");
                    var num = Console.ReadLine();

                    if (!int.TryParse(num, out int numberChosen) || numberChosen < 0 || numberChosen >= games.Count)
                    {
                        Console.WriteLine("That is an invalid input...");
                        continue;
                    }

                    var gameToTest = games[numberChosen];

                    Console.WriteLine("You have chosen to test: " + gameToTest.GameName);
                    Console.Write("How long (in seconds) would you like the test to run for?: ");

                    var num2 = Console.ReadLine();
                    if (!int.TryParse(num2, out int secondsChosen) || secondsChosen <= 0)
                    {
                        Console.WriteLine("That is an invalid input...");
                        continue;
                    }


                    DateTime finishTime = DateTime.Now.AddSeconds(secondsChosen);

                    Console.WriteLine("Test will run until: " + finishTime.ToLongTimeString());

                    Console.WriteLine("Press any key to begin");
                    Console.ReadKey();

                    Console.WriteLine("Running test....");

                    var results = testExecutor.RunTest(gameToTest, finishTime);
                    Console.WriteLine("A total of: " + results.totalActionsDone + " actions were made in the test");

                    Console.WriteLine("### Errors ###");
                    results.errors.ForEach(a => { Console.WriteLine(a); Console.WriteLine(); });
                    Console.WriteLine("### Warnings ###");
                    results.warnings.ForEach(a => { Console.WriteLine(a); Console.WriteLine(); });

                    reporter.ReportMessage("--- Test report for game: " + gameToTest.GameName + " ---");
                    reporter.ReportMessage("A total of: " + results.totalActionsDone + " actions were made in the test");
                    reporter.ReportMessage("~~~~~~~~~ ERRORS ~~~~~~~~~");
                    results.errors.ForEach(a => { reporter.ReportMessage(a); });
                    reporter.ReportMessage("~~~~~~~~~ WARNINGS ~~~~~~~~~");
                    results.warnings.ForEach(a => { reporter.ReportMessage(a); });
                    reporter.ReportMessage("~~~~~~~~~ End of report ~~~~~~~~~");

                    Console.WriteLine("Finished!");

                    Console.ReadLine();



                }
            }
        }
    }
}
