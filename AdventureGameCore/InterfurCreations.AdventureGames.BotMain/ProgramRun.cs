using System;
using System.Threading;
using System.Reflection;
using InterfurCreations.AdventureGames.Configuration;
using InterfurCreations.AdventureGames.Services;
using InterfurCreations.AdventureGames.SlackReporter;
using InterfurCreations.AdventureGames.DatabaseServices.Interfaces;
using InterfurCreations.AdventureGames.Services.Interfaces;
using InterfurCreations.AdventureGames.Telegram;
using InterfurCreations.AdventureGames.Core;
using InterfurCreations.AdventureGames.GameLanguage;
using InterfurCreations.AdventureGames.Core.Interface;
using InterfurCreations.AdventureGames.DatabaseServices;
using InterfurCreations.AdventureGames.Graph;
using InterfurCreations.AdventureGames.Discord;
using InterfurCreations.AdventureGames.HeartbeatMonitor;
using Autofac;
using InterfurCreations.AdventureGames.Logging;
using InterfurCreations.AdventureGames.Graph.Store;
using Microsoft.Extensions.Configuration;
using InterfurCreations.AdventureGames.Services.ImageStore;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Autofac.Extensions.DependencyInjection;
using InterfurCreations.AdventureGames.OpenAI;

namespace InterfurCreations.AdventureGames.BotMain
{
    public class ProgramRun
    {
        private static ManualResetEvent _quitEvent = new ManualResetEvent(false);
        private static bool IsInConsoleMode = false;
        private static IContainer Container { get; set; }

        public static void Main(string[] args)
        {
            Console.CancelKeyPress += (sender, eArgs) => {
                _quitEvent.Set();
                eArgs.Cancel = true;
            };

            IsInConsoleMode = true;

            Run(null);
        }

        public static void Run(IConfiguration config)
        {
            string buildTypeName = null;
#if TelegramDev
            buildTypeName = "Telegram Dev";
#elif TelegramAlpha
            buildTypeName = "Telegram Alpha";
#elif TelegramLive
            buildTypeName = "Telegram Live";
#elif DiscordAlpha
            buildTypeName = "Discord Alpha";
#elif DiscordLive
            buildTypeName = "Discord Live";
#elif KikLive
            buildTypeName = "Kik Live";
#endif
            ConfigSetting.DynamicApplicationName = buildTypeName;
            var configSetupService = new AppSettingsConfigurationService(config);
            configSetupService.SetConfig("TypeName", buildTypeName);

            RegisterAutofac(config, buildTypeName);
            

            var connectionString = configSetupService.GetConfig("DatabaseConnectionString");

            ContainerStore.Container = Container;
            ContainerStore.ServiceProvider = new AutofacServiceProvider(Container);

            ServiceStore.HttpLockObj = new object();

            using (var scope = Container.BeginLifetimeScope())
            {

                var configService = scope.Resolve<IConfigurationService>();

#if !TelegramDev
                    Log.EnableReporting(scope.Resolve<IReporter>());
#endif
                // List games straight away, so there is no long delay when the first person sends a message
                scope.Resolve<IGameRetrieverService>().ListGames();

                var inputController = scope.Resolve<IInputController>();
                inputController.Setup();

                scope.Resolve<IHeartbeatMonitor>().BeginMonitor(configService.GetConfigOrDefault("HeartbeatUrl", null, true));


                if (IsInConsoleMode)
                    _quitEvent.WaitOne();
            }
        }

        public static void RegisterAutofac(IConfiguration config, string buildTypeName)
        {
            var builder = new ContainerBuilder();
            builder.RegisterInstance<IConfiguration>(config);

            builder.RegisterType<AppSettingsConfigurationService>().As<IConfigurationService>().SingleInstance();
            builder.RegisterType<InputController>().As<IInputController>().SingleInstance();
            builder.RegisterType<SlackReport>().As<IReporter>().SingleInstance();
            builder.RegisterType<SlackReportGenerator>().As<ISlackReportGenerator>().SingleInstance();
            builder.RegisterType<TelegramCredentialsProvider>().As<ICredentialsPathProvider>().SingleInstance();
            builder.RegisterType<ConfigSettingsGoogleDriveAuthenticator>().As<IGoogleDriveAuthenticator>().SingleInstance();
            builder.RegisterType<GoogleDriveService>().As<IGoogleDriveService>().SingleInstance();
            builder.RegisterType<GameExecutor>().As<IGameExecutor>().InstancePerLifetimeScope();
            builder.RegisterType<GameProcessor>().As<IGameProcessor>().InstancePerLifetimeScope();
            builder.RegisterType<TextParsing>().As<ITextParsing>().InstancePerLifetimeScope();
            builder.RegisterType<GameDataService>().As<IGameDataService>().InstancePerLifetimeScope();
            builder.RegisterType<DatabaseDataStore>().As<IDataStore>().InstancePerLifetimeScope();
            builder.RegisterType<AccessService>().As<IAccessService>().InstancePerLifetimeScope();
            builder.RegisterType<AccessTokenService>().As<IAccessTokenService>().InstancePerLifetimeScope();
            builder.RegisterType<TokenGenerator>().As<ITokenGenerator>().InstancePerLifetimeScope();
            builder.RegisterType<HeartbeatMonitorService>().As<IHeartbeatMonitor>().InstancePerLifetimeScope();
            builder.RegisterType<ImagingService>().As<IImagingService>().SingleInstance();
            builder.RegisterType<AwsImageStore>().As<IImageStore>().InstancePerLifetimeScope();
            builder.RegisterType<LanguageToolSpellChecker>().As<ISpellChecker>().InstancePerLifetimeScope();
            builder.RegisterType<ImageBuildDataTracker>().InstancePerLifetimeScope();
            builder.RegisterType<StatisticsService>().As<IStatisticsService>().InstancePerLifetimeScope();

            builder.RegisterType<DatabaseContextProvider>().As<IDatabaseContextProvider>().InstancePerLifetimeScope();
            builder.RegisterType<AccountController>().As<IAccountController>().InstancePerLifetimeScope();
            builder.RegisterType<GameSaveService>().As<IGameSaveService>().InstancePerLifetimeScope();

            builder.RegisterType<PlayerDatabaseController>().As<IPlayerDatabaseController>().InstancePerLifetimeScope();
            builder.RegisterType<DrawStore>().As<IGameStore>().SingleInstance();
            builder.RegisterType<GameRetrieverService>().As<IGameRetrieverService>().SingleInstance();

            builder.RegisterType<ImageStoreCleanupTask>().InstancePerLifetimeScope();

            builder.RegisterType<OpenAIConnector>().As<IOpenAIConnector>().SingleInstance();
            builder.RegisterType<ChatGptService>().As<IAITextService>().SingleInstance();


            builder.RegisterAssemblyTypes(typeof(IMessageHandler).Assembly)
                .AssignableTo<IMessageHandler>()
                .AsImplementedInterfaces();

            if (buildTypeName.ToLower().StartsWith("telegram"))
                RegisterTelegram(builder);
            else if (buildTypeName.ToLower().StartsWith("discord"))
                RegisterDiscord(builder);
            //else if(buildTypeName.ToLower().StartsWith("kik"))
            //    RegisterKik(builder);
            Container = builder.Build();
        }

        public static void RegisterTelegram(ContainerBuilder builder)
        {
            builder.RegisterType<TelegramCommunicator>().As<ICommunicator>().InstancePerLifetimeScope();
            builder.RegisterType<TelegramBotOverseer>().As<TelegramBotOverseer>().InstancePerLifetimeScope();
        }

        public static void RegisterDiscord(ContainerBuilder builder)
        {
            builder.RegisterType<DiscordBotCommunicator>().As<ICommunicator>().InstancePerLifetimeScope();
            builder.RegisterType<DiscordMessageHandler>().As<DiscordMessageHandler>().InstancePerLifetimeScope();
        }

        //public static void RegisterKik(ContainerBuilder builder)
        //{
        //    builder.RegisterType<WebhookMessageHandlerService>().As<IWebhookMessageHandlerService>().InstancePerLifetimeScope();
        //    builder.RegisterType<WebhookRunService>().As<IWebhookService>().InstancePerLifetimeScope();
        //    builder.RegisterType<KikService>().As<ICommunicator>().InstancePerLifetimeScope();
        //    builder.RegisterType<KikMessageExecutor>().As<KikMessageExecutor>().InstancePerLifetimeScope();

        //    var _ = typeof(Startup);

        //    var webAssembly = Assembly.GetExecutingAssembly();
        //    var repoAssembly = Assembly.GetAssembly(typeof(KikService));
        //    builder.RegisterAssemblyTypes(webAssembly, repoAssembly)
        //                .AsImplementedInterfaces();
        //}
    }
}
