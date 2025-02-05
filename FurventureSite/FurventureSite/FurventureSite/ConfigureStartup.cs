using Autofac;
using InterfurCreations.AdventureGames.Configuration;
using InterfurCreations.AdventureGames.Core;
using InterfurCreations.AdventureGames.Core.Interface;
using InterfurCreations.AdventureGames.DatabaseServices;
using InterfurCreations.AdventureGames.DatabaseServices.Interfaces;
using InterfurCreations.AdventureGames.GameLanguage;
using InterfurCreations.AdventureGames.Graph.Store;
using InterfurCreations.AdventureGames.Logging;
using InterfurCreations.AdventureGames.OpenAI;
using InterfurCreations.AdventureGames.Services;
using InterfurCreations.AdventureGames.Services.ImageStore;
using InterfurCreations.AdventureGames.Services.Interfaces;
using InterfurCreations.AdventureGames.SlackReporter;


namespace FurventureSite
{
    public class ConfigureStartup
    {
        public IConfiguration Configuration { get; }
        public IContainer AutofacContainer { get; set; }

        public ConfigureStartup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            RegisterAutofac(builder);
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        public void RegisterAutofac(ContainerBuilder builder)
        {
            //var builder = new ContainerBuilder();
            builder.RegisterType<AppSettingsConfigurationService>().As<IConfigurationService>().InstancePerLifetimeScope();
            builder.RegisterType<DatabaseContextProvider>().As<IDatabaseContextProvider>().InstancePerLifetimeScope();
            builder.RegisterType<AccountController>().As<IAccountController>().InstancePerLifetimeScope();
            builder.RegisterType<PlayerDatabaseController>().As<IPlayerDatabaseController>().InstancePerLifetimeScope();
            builder.RegisterType<ConfigSettingsGoogleDriveAuthenticator>().As<IGoogleDriveAuthenticator>().InstancePerLifetimeScope();
            builder.RegisterType<GoogleDriveService>().As<IGoogleDriveService>().InstancePerLifetimeScope();
            builder.RegisterType<AccessTokenService>().As<IAccessTokenService>().InstancePerLifetimeScope();
            builder.RegisterType<TokenGenerator>().As<ITokenGenerator>().InstancePerLifetimeScope();
            builder.RegisterType<SlackReport>().As<IReporter>().InstancePerLifetimeScope();
            builder.RegisterType<StatisticsService>().As<IStatisticsService>().InstancePerLifetimeScope();

            builder.RegisterType<ImagingService>().As<IImagingService>().SingleInstance();
            builder.RegisterType<AwsImageStore>().As<IImageStore>().InstancePerLifetimeScope();
            builder.RegisterType<LanguageToolSpellChecker>().As<ISpellChecker>().InstancePerLifetimeScope();
            builder.RegisterType<ImageBuildDataTracker>().InstancePerLifetimeScope();
            builder.RegisterType<ImageStoreCleanupTask>().InstancePerLifetimeScope();

            builder.RegisterType<GameExecutor>().As<IGameExecutor>().InstancePerLifetimeScope();
            builder.RegisterType<GameProcessor>().As<IGameProcessor>().InstancePerLifetimeScope();
            builder.RegisterType<TextParsing>().As<ITextParsing>().InstancePerLifetimeScope();
            builder.RegisterType<GameDataService>().As<IGameDataService>().InstancePerLifetimeScope();
            builder.RegisterType<DatabaseDataStore>().As<IDataStore>().InstancePerLifetimeScope();
            builder.RegisterType<AccessService>().As<IAccessService>().InstancePerLifetimeScope();
            builder.RegisterType<TokenGenerator>().As<ITokenGenerator>().InstancePerLifetimeScope();
            builder.RegisterType<GameSaveService>().As<IGameSaveService>().InstancePerLifetimeScope();
            builder.RegisterType<ImagingService>().As<IImagingService>().InstancePerLifetimeScope();
            builder.RegisterType<ImageBuildDataTracker>().InstancePerLifetimeScope();

            builder.RegisterType<DrawStore>().As<IGameStore>().SingleInstance();
            builder.RegisterType<GameRetrieverService>().As<IGameRetrieverService>().SingleInstance();

            builder.RegisterType<OpenAIConnector>().As<IOpenAIConnector>().SingleInstance();
            builder.RegisterType<ChatGptService>().As<IAITextService>().SingleInstance();

            builder.RegisterAssemblyTypes(typeof(IMessageHandler).Assembly)
                .AssignableTo<IMessageHandler>()
                .AsImplementedInterfaces();

            var buildTypeName = Environment.GetEnvironmentVariable("FurventureBotType");

            ConfigSetting.DynamicApplicationName = buildTypeName;
        }
    }
}
