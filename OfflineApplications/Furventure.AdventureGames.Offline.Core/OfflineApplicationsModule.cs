using Autofac;
using Furventure.AdventureGames.DatabaseServices.Offline;
using InterfurCreations.AdventureGames.Configuration;
using InterfurCreations.AdventureGames.Core;
using InterfurCreations.AdventureGames.Core.Interface;
using InterfurCreations.AdventureGames.DatabaseServices;
using InterfurCreations.AdventureGames.DatabaseServices.Interfaces;
using InterfurCreations.AdventureGames.GameLanguage;
using InterfurCreations.AdventureGames.Graph.Store;
using InterfurCreations.AdventureGames.Logging;
using InterfurCreations.AdventureGames.Services.ImageStore;
using InterfurCreations.AdventureGames.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Furventure.AdventureGames.Offline.Core
{
    public class OfflineApplicationsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<OfflineMessageProcessor>();
            builder.RegisterType<GameExecutor>().As<IGameExecutor>();
            builder.RegisterType<OfflinePlayerController>().As<IOfflinePlayerController>();
            builder.RegisterType<EmptyReporter>().As<IReporter>().SingleInstance();
            builder.RegisterType<GameProcessor>().As<IGameProcessor>().InstancePerLifetimeScope();
            builder.RegisterType<TextParsing>().As<ITextParsing>().InstancePerLifetimeScope();
            builder.RegisterType<GameDataService>().As<IGameDataService>().InstancePerLifetimeScope();
            builder.RegisterType<ImagingService>().As<IImagingService>().SingleInstance();
            builder.RegisterType<ImageBuildDataTracker>().InstancePerLifetimeScope();
            builder.RegisterType<GameRetrieverService>().As<IGameRetrieverService>().SingleInstance();
            builder.RegisterType<OfflineDatabaseContextProvider>().As<IDatabaseContextProvider>().InstancePerLifetimeScope();
            builder.RegisterType<DatabaseDataStore>().As<IDataStore>().InstancePerLifetimeScope();
            builder.RegisterType<AllAllowedAccessService>().As<IAccessService>().InstancePerLifetimeScope();
            builder.RegisterType<EmptyStatisticsService>().As<IStatisticsService>().InstancePerLifetimeScope();
            builder.RegisterType<GameFileDrawStore>().As<IGameStore>().SingleInstance();
            builder.RegisterType<AppSettingsConfigurationService>().As<IConfigurationService>().SingleInstance();
            builder.RegisterType<MemoryImageStore>().As<IImageStore>().InstancePerLifetimeScope();
            builder.RegisterType<GameSaveService>().As<IGameSaveService>().InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(typeof(IMessageHandler).Assembly)
                    .AssignableTo<IMessageHandler>()
                    .AsImplementedInterfaces();

            //builder.RegisterType<AccessTokenService>().As<IAccessTokenService>().InstancePerLifetimeScope();
            //builder.RegisterType<TokenGenerator>().As<ITokenGenerator>().InstancePerLifetimeScope();
        }
    }
}
