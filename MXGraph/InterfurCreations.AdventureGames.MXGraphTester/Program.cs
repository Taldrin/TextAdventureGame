using Autofac;
using InterfurCreations.AdventureGames.Configuration;
using InterfurCreations.AdventureGames.DatabaseServices;
using InterfurCreations.AdventureGames.DatabaseServices.Interfaces;
using InterfurCreations.AdventureGames.Graph.Store;
using InterfurCreations.AdventureGames.GraphServices;
using InterfurCreations.AdventureGames.Logging;
using InterfurCreations.AdventureGames.MXGraphDrawServices;
using InterfurCreations.AdventureGames.MXVisualGraph;
using InterfurCreations.AdventureGames.Services;
using InterfurCreations.AdventureGames.Services.Interfaces;
using System;
using System.IO;

namespace InterfurCreations.AdventureGames.MXGraphTester
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Let's Begin");

            var container = RunAutofac();

            using (var scope = container.BeginLifetimeScope())
            {
                var playerDbController = scope.Resolve<IPlayerDatabaseController>();
                var gameStore = scope.Resolve<IGameStore>();

                var player = playerDbController.GetPlayerById("0e11eec8-739e-4e23-8738-6ff81610369d ");
                PlayerGraphGenerator gen = new PlayerGraphGenerator(player, gameStore);

                var game = gen.GenerateLastGame();

                var converter = new DrawGameConverter();
                var graph = converter.ConvertToMXGraph(game);

                var generator = new MXGraphXMLGenerator();
                var output = generator.Generate(graph);

                File.WriteAllText("C:\\Users\\max\\Desktop\\testGraph.xml", output);

                Console.WriteLine(output);
                Console.ReadKey();
            }
        }

        private static IContainer RunAutofac()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<ConfigurationService>().As<IConfigurationService>().SingleInstance();
            builder.RegisterType<ConsoleAppGoogleDriveAuthenticator>().As<IGoogleDriveAuthenticator>().SingleInstance();
            builder.RegisterType<GoogleDriveService>().As<IGoogleDriveService>().SingleInstance();
            builder.RegisterType<DatabaseContextProvider>().As<IDatabaseContextProvider>().InstancePerLifetimeScope();
            builder.RegisterType<PlayerDatabaseController>().As<IPlayerDatabaseController>().InstancePerLifetimeScope();
            builder.RegisterType<DrawStore>().As<IGameStore>().SingleInstance();
            builder.RegisterType<EmptyReporter>().As<IReporter>().SingleInstance();
            return builder.Build();
        }
    }
}
