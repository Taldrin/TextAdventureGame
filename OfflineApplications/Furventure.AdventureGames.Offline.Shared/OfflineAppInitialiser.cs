using Autofac;
using Furventure.AdventureGames.Offline.Core;
using Furventure.AdventureGames.Offline.Database;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Furventure.AdventureGames.Offline
{
    public class OfflineAppInitialiser
    {
        public static IContainer Container;
        private static IConfiguration config;

        internal void Initialise()
        {
            config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

            RegisterAutofac();

            new OfflineDatabaseContext().Database.EnsureCreated();

            //var input = "start";
            //while (true)
            //{
            //    using (var scope = Container.BeginLifetimeScope())
            //    {
            //        var messageProcessor = scope.Resolve<OfflineMessageProcessor>();
            //        var result = messageProcessor.ProcessMessage(input);

            //    }
            //}
        }

        public static void RegisterAutofac()
        {
            var builder = new ContainerBuilder();
            builder.RegisterInstance(config);

            builder.RegisterModule(new OfflineApplicationsModule());

            Container = builder.Build();
        }
    }
}
