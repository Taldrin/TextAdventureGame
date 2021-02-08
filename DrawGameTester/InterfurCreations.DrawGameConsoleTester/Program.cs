using Autofac;
using InterfurCreations.AdventureGames.Configuration;
using InterfurCreations.AdventureGames.Core;
using InterfurCreations.AdventureGames.Core.Interface;
using InterfurCreations.AdventureGames.GameLanguage;
using InterfurCreations.AdventureGames.Graph;
using InterfurCreations.AdventureGames.Graph.Store;
using InterfurCreations.AdventureGames.Logging;
using InterfurCreations.AdventureGames.Services;
using InterfurCreations.AdventureGames.Services.ImageStore;
using InterfurCreations.AdventureGames.Services.Interfaces;
using InterfurCreations.AdventureGames.SlackReporter;
using InterfurCreations.AdventureGames.Tester.DrawIOTest;
using InterfurCreations.DrawGameConsoleTester;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawGameTester
{
    public class Program
    {
        private static IContainer Container { get; set; }

        public static async Task Main(string[] args)
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<TextParsing>().As<ITextParsing>().InstancePerLifetimeScope();
            builder.RegisterType<GameDataService>().As<IGameDataService>().InstancePerLifetimeScope();
            builder.RegisterType<GameProcessor>().As<IGameProcessor>().InstancePerLifetimeScope();
            builder.RegisterType<DrawStore>().As<IGameStore>().SingleInstance();
            builder.RegisterType<ConfigSettingsGoogleDriveAuthenticator>().As<IGoogleDriveAuthenticator>().SingleInstance();
            builder.RegisterType<GoogleDriveService>().As<IGoogleDriveService>().SingleInstance();
            builder.RegisterType<AppSettingsConfigurationService>().As<IConfigurationService>().SingleInstance();
            builder.RegisterType<DrawGameTestExecutor>().As<DrawGameTestExecutor>().SingleInstance();
            builder.RegisterType<AzureSpellChecker>().As<ISpellChecker>().SingleInstance();
            builder.RegisterType<EmptyImagineService>().As<IImagingService>().SingleInstance();
            builder.RegisterType<ImageBuildDataTracker>().As<ImageBuildDataTracker>().SingleInstance();

            Console.WriteLine("Game Tester");
            Console.Write("Output to Slack or File? (Slack/File): ");

            var connection = Environment.GetEnvironmentVariable("ConnectionString");

            IConfiguration Configuration = new ConfigurationBuilder()
               .AddAzureAppConfiguration(connection)
               .AddJsonFile("appsettings.json")
                .Build();

            var configSetupService = new AppSettingsConfigurationService(Configuration);
            configSetupService.SetConfig("TypeName", "GameTester");
            ConfigSetting.DynamicApplicationName = "GameTester";

            builder.RegisterInstance(Configuration);

            var line = Console.ReadLine();

            if (line.ToLower() == "slack")
            {
                builder.RegisterType<SlackReport>().As<IReporter>().SingleInstance();
            }
            else
            {
                builder.RegisterType<FileReporter>().As<IReporter>().SingleInstance();
            }

            Container = builder.Build();

            while (true)
            {

                using (var scope = Container.BeginLifetimeScope())
                {
                    var drawStore = scope.Resolve<IGameStore>();
                    var testExecutor = scope.Resolve<DrawGameTestExecutor>();
                    var reporter = scope.Resolve<IReporter>();
                    var spellChecker = scope.Resolve<ISpellChecker>();
                    reporter.Initialise();
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

                    Console.Write("How many actions per run? (0 for unlimited): ");
                    var actionsPerRunString = Console.ReadLine();
                    if (!int.TryParse(actionsPerRunString, out int actionsPerRun))
                    {
                        Console.WriteLine("That is an invalid input...");
                        continue;
                    }

                    Console.Write("Enter state ID of the starting state to test (enter nothing to use the games start state): ");
                    var stateId = Console.ReadLine();

                    DateTime finishTime = DateTime.Now.AddSeconds(secondsChosen);

                    Console.WriteLine("Test will run until: " + finishTime.ToLongTimeString());

                    Console.WriteLine("Press any key to begin");
                    Console.ReadKey();

                    Console.WriteLine("Running test....");

                    var results = await testExecutor.RunTestAsync(gameToTest, finishTime, actionsPerRun, stateId);
                    Console.WriteLine("A total of: " + results.totalActionsDone + " actions were made in the test");

                    Console.WriteLine("### Errors ###");
                    results.errors.ForEach(a => { Console.WriteLine(a); Console.WriteLine(); });
                    Console.WriteLine("### Warnings ###");
                    results.warnings.ForEach(a => { Console.WriteLine(a); Console.WriteLine(); });

                    reporter.ReportMessage("--- Test report for game: " + gameToTest.GameName + " ---");
                    reporter.ReportMessage("~~~~~~~~~ ERRORS ~~~~~~~~~");
                    results.errors.ForEach(a => { reporter.ReportMessage(a); });
                    reporter.ReportMessage("~~~~~~~~~ WARNINGS ~~~~~~~~~");
                    results.warnings.ForEach(a => { reporter.ReportMessage(a); });
                    reporter.ReportMessage("~~~~~~~~~ End of report ~~~~~~~~~");
                    reporter.ReportMessage("A total of: " + results.totalActionsDone + " actions were made in the test");

                    Console.WriteLine("Check spelling? (Y/N): ");
                    if (Console.ReadLine().ToLower() == "y")
                    {
                        Console.WriteLine("Running spell checks on: " + results.allText.Count + " messages");
                        await RunSpellingAsync(results.allText, spellChecker, reporter);
                    }

                    Console.WriteLine("Finished!");
                    Console.ReadLine();
                }
            }
        }

        private static async Task RunSpellingAsync(List<string> allText, ISpellChecker spellChecker, IReporter reporter)
        {
            int total = allText.Count;
            int current = 0;
            int lastPercent = 0;
            foreach (var message in allText)
            {
                SpellCheckResult spellCheckResult = null;
                try
                {
                    spellCheckResult = await spellChecker.CheckSpellingAsync(message);
                }
                catch (Exception e) { Console.WriteLine(e.Message); }

                if (spellCheckResult != null && spellCheckResult.suggestions != null && spellCheckResult.suggestions.Any())
                {
                    foreach (var spellSuggest in spellCheckResult.suggestions)
                    {
                        var reportText = ReportSpelling($"Word: '{spellSuggest.original}' - did you mean: '{spellSuggest.suggestion}'? Character number: {spellSuggest.wordLocation}", message);
                        reporter.ReportMessage(reportText);
                        Console.WriteLine(reportText);
                    }
                }
                current++;

                var percent = (int)((double)current / total * 100.0);
                if (percent > lastPercent)
                {
                    lastPercent = percent;
                    Console.WriteLine($"{percent}% Spell Checked");
                }
            }
        }

        private static string ReportSpelling(string text, string stateText)
        {
            var mText = $"{text} \nIn state with text: '{(!string.IsNullOrWhiteSpace(stateText) && stateText.Length >= 300 ? stateText.Substring(0, 300) : stateText)}'";
            return mText;
        }
    }
}
