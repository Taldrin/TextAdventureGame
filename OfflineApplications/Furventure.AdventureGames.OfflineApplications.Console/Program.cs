using Autofac;
using Autofac.Core;
using Furventure.AdventureGames.Offline.Core;
using Furventure.AdventureGames.Offline.Database;
using InterfurCreations.AdventureGames.DatabaseServices.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Spectre.Console;
using System;
using System.Linq;

namespace Furventure.AdventureGames.OfflineApplications.Console
{
    class Program
    {
        private static IContainer container;
        private static IConfiguration config;

        static void Main(string[] args)
        {
            config = new ConfigurationBuilder()
                     .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                     .AddEnvironmentVariables()
                     .AddCommandLine(args)
                     .Build();

            RegisterAutofac();

            new OfflineDatabaseContext().Database.EnsureCreated();

            var input = "start";
            while (true)
            {
                using (var scope = container.BeginLifetimeScope())
                {
                    var messageProcessor = scope.Resolve<OfflineMessageProcessor>();
                    var result = messageProcessor.ProcessMessage(input);

                    var message = string.Join("\n\n", result.MessagesToShow.Select(a => a.Message));
                    message = message.Replace("[", "(").Replace("]", ")");

                    input = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title($"[bold]{message}[/]")
                            .PageSize(10)
                            .MoreChoicesText("[grey](Scroll up or down for more options)[/]")
                            .AddChoices(result.OptionsToShow));
                    scope.Resolve<IDatabaseContextProvider>().GetContext().SaveChanges();
                }
            }
        }

        public static void RegisterAutofac()
        {
            var builder = new ContainerBuilder();
            builder.RegisterInstance(config);

            builder.RegisterModule(new OfflineApplicationsModule());

            container = builder.Build();
        }
    }
}
