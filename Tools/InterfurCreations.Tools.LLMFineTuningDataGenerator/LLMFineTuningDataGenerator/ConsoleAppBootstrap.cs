using Autofac;
using InterfurCreations.AdventureGames.Configuration;
using InterfurCreations.AdventureGames.Core;
using InterfurCreations.AdventureGames.Core.Interface;
using InterfurCreations.AdventureGames.GameLanguage;
using InterfurCreations.AdventureGames.GameTesting;
using InterfurCreations.AdventureGames.Graph.Store;
using InterfurCreations.AdventureGames.Logging;
using InterfurCreations.AdventureGames.Services;
using InterfurCreations.AdventureGames.Services.ImageStore;
using InterfurCreations.AdventureGames.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLMFineTuningDataGenerator
{
    public class ConsoleAppBootstrap
    {
        public IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<TextParsing>().As<ITextParsing>().InstancePerLifetimeScope();
            builder.RegisterType<GameDataService>().As<IGameDataService>().InstancePerLifetimeScope();
            builder.RegisterType<GameProcessor>().As<IGameProcessor>().InstancePerLifetimeScope();
            builder.RegisterType<DrawStore>().As<IGameStore>().SingleInstance();
            builder.RegisterType<GameRetrieverService>().As<IGameRetrieverService>().SingleInstance();
            builder.RegisterType<ConfigSettingsGoogleDriveAuthenticator>().As<IGoogleDriveAuthenticator>().SingleInstance();
            builder.RegisterType<GoogleDriveService>().As<IGoogleDriveService>().SingleInstance();
            builder.RegisterType<AppSettingsConfigurationService>().As<IConfigurationService>().SingleInstance();
            builder.RegisterType<DrawGameTestExecutor>().As<DrawGameTestExecutor>().SingleInstance();
            builder.RegisterType<AzureSpellChecker>().As<ISpellChecker>().SingleInstance();
            builder.RegisterType<EmptyImagineService>().As<IImagingService>().SingleInstance();
            builder.RegisterType<ImageBuildDataTracker>().As<ImageBuildDataTracker>().SingleInstance();
            builder.RegisterType<ConsoleReporter>().As<IReporter>().SingleInstance();
            builder.RegisterType<GameTestExecutor>().As<GameTestExecutor>().InstancePerLifetimeScope();

            var connection = Environment.GetEnvironmentVariable("ConnectionString");

            IConfiguration Configuration = new ConfigurationBuilder()
   .AddAzureAppConfiguration(connection)
   .AddJsonFile("appsettings.json")
    .Build();

            var configSetupService = new AppSettingsConfigurationService(Configuration);
            configSetupService.SetConfig("TypeName", "GameTester");
            ConfigSetting.DynamicApplicationName = "GameTester";

            builder.RegisterInstance(Configuration);

            return builder.Build();
        }
    }
}
