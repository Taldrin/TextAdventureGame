using Hangfire;
using Hangfire.Logging;
using Hangfire.Logging.LogProviders;
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
using InterfurCreations.AdventureGames.Kik;
using InterfurCreations.AdventureGames.HeartbeatMonitor;
using Autofac;
using InterfurCreations.AdventureGames.Logging;
using InterfurCreations.AdventureGames.Graph.Store;

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

            Run();
        }

        public static void Run()
        {
            RegisterAutofac();

            var connectionString = new ConfigurationService().GetConfig("DatabaseConnectionString");

            GlobalConfiguration.Configuration.UseSqlServerStorage(connectionString);
            LogProvider.SetCurrentLogProvider(new ColouredConsoleLogProvider());
            GlobalConfiguration.Configuration.UseAutofacActivator(Container);

            ContainerStore.Container = Container;

            ServiceStore.HttpLockObj = new object();

            using (var scope = Container.BeginLifetimeScope())
            {
                using (var server = new BackgroundJobServer())
                {
                    var configService = scope.Resolve<IConfigurationService>();
                    Log.EnableReporting(scope.Resolve<IReporter>());
                    HangfireReporter report = new HangfireReporter();
                    report.SetupJobs(scope.Resolve<IDatabaseContextProvider>(), scope.Resolve<IReporter>());
                    var inputController = scope.Resolve<IInputController>();
                    inputController.Setup();

                    scope.Resolve<IHeartbeatMonitor>().BeginMonitor(configService.GetConfigOrDefault("HeartbeatUrl", null));

                    if(IsInConsoleMode)
                        _quitEvent.WaitOne();
                }
            }
        }

        public static void RegisterAutofac()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<ConfigurationService>().As<IConfigurationService>().SingleInstance();
            builder.RegisterType<InputController>().As<IInputController>().SingleInstance();
            builder.RegisterType<SlackReport>().As<IReporter>().SingleInstance();
            builder.RegisterType<SlackReportGenerator>().As<ISlackReportGenerator>().SingleInstance();
            builder.RegisterType<TelegramCredentialsProvider>().As<ICredentialsPathProvider>().SingleInstance();
            builder.RegisterType<ConsoleAppGoogleDriveAuthenticator>().As<IGoogleDriveAuthenticator>().SingleInstance();
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

            builder.RegisterType<DatabaseContextProvider>().As<IDatabaseContextProvider>().InstancePerLifetimeScope();
            builder.RegisterType<PlayerDatabaseController>().As<IPlayerDatabaseController>().InstancePerLifetimeScope();
            builder.RegisterType<AccountController>().As<IAccountController>().InstancePerLifetimeScope();
            builder.RegisterType<GameSaveService>().As<IGameSaveService>().InstancePerLifetimeScope();


            builder.RegisterType<DrawStore>().As<IGameStore>().SingleInstance();

            builder.RegisterAssemblyTypes(typeof(IMessageHandler).Assembly)
                .AssignableTo<IMessageHandler>()
                .AsImplementedInterfaces();

            var type = new ConfigurationService().GetConfig("TypeName");

            if (type.ToLower().Contains("telegram"))
                RegisterTelegram(builder);
            else if (type.ToLower().Contains("discord"))
                RegisterDiscord(builder);
            else if(type.ToLower().Contains("kik"))
                RegisterKik(builder);
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

        public static void RegisterKik(ContainerBuilder builder)
        {
          //  builder.RegisterType<WebhookMessageHandlerService>().As<IWebhookMessageHandlerService>().InstancePerLifetimeScope();
          //  builder.RegisterType<WebhookRunService>().As<IWebhookService>().InstancePerLifetimeScope();
            builder.RegisterType<KikService>().As<ICommunicator>().InstancePerLifetimeScope();
            builder.RegisterType<KikMessageExecutor>().As<KikMessageExecutor>().InstancePerLifetimeScope();

            // Assembly scanning will not pickup the Webhook project unless it's been references
           // var _ = typeof(Startup);

            var webAssembly = Assembly.GetExecutingAssembly();
            var repoAssembly = Assembly.GetAssembly(typeof(KikService));
            builder.RegisterAssemblyTypes(webAssembly, repoAssembly)
                        .AsImplementedInterfaces();
        }
    }
}
