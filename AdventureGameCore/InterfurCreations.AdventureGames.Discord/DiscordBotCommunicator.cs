using InterfurCreations.AdventureGames.Configuration;
using InterfurCreations.AdventureGames.Services;
using InterfurCreations.AdventureGames.Services.Interfaces;
using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using InterfurCreations.AdventureGames.DatabaseServices.Interfaces;
using Discord;
using InterfurCreations.AdventureGames.Core.Interface;
using Autofac;
using System.Threading;
using InterfurCreations.AdventureGames.Logging;

namespace InterfurCreations.AdventureGames.Discord
{
    public class DiscordBotCommunicator : ICommunicator
    {
        private readonly IReporter _reporter;
        private readonly IAccountController _accountController;
        private readonly IPlayerDatabaseController _playerDatabaseController;
        private readonly IGameExecutor _gameExecutor;
        private readonly IConfigurationService _configService;

        private DiscordSocketClient socketClient;

        public DiscordBotCommunicator(IReporter reporter, IGoogleDriveService gdriveService, IAccountController accountController, IPlayerDatabaseController playerDatabaseController,
            IGameExecutor gameExecutor, IConfigurationService configurationService)
        {
            _reporter = reporter;
            _accountController = accountController;
            _playerDatabaseController = playerDatabaseController;
            _gameExecutor = gameExecutor;
            _configService = configurationService;

            _reporter.Initialise();
        }

        public async void SetupAsync()
        {
            socketClient = new DiscordSocketClient();
            socketClient.Log += SocketClient_Log;

            var token = _configService.GetConfig("DiscordApiToken", true);

            await socketClient.LoginAsync(TokenType.Bot, token);
            await socketClient.StartAsync();

            socketClient.MessageReceived += MessageExecutor;
            socketClient.Disconnected += SocketClient_Disconnected;
            await socketClient.SetGameAsync("ft.start", null, ActivityType.Playing);

        }

        private async Task SocketClient_Disconnected(Exception arg)
        {
            try
            {
                await socketClient.LoginAsync(TokenType.Bot, _configService.GetConfig("DiscordApiToken", true));
                socketClient.StartAsync();
            }
            catch (Exception e)
            {
                _reporter.ReportError("Error trying to logout: " + e.Message);
            }
        }

        private Task SocketClient_Log(LogMessage arg)
        {
            return Task.Run(() => Log.LogMessage(arg.Message, LogType.General));
        }

        private async Task MessageExecutor(SocketMessage arg)
        {
            try
            {
                if (arg.Author.IsBot) return;


                // Filter out any messages that don't start with 'ft.'
                if (!arg.Content.StartsWith("ft.") && !(arg.Channel is SocketDMChannel)) return;

                if (arg.Channel is ITextChannel textChannel && !(arg.Channel is SocketDMChannel) && !textChannel.IsNsfw)
                {
                    await arg.Channel.SendMessageAsync("OwO? This bot contains adult content, " +
                        "and so must be used either in a NSFW channel, or privately messaged!");
                    return;
                }

                using (var scope = ContainerStore.Container.BeginLifetimeScope())
                {
                    var discordMessageHandler = scope.Resolve<DiscordMessageHandler>();
                    await discordMessageHandler.MessageReceived(arg);
                }
            } catch (Exception e)
            {
                Log.LogMessage(e.Message, LogType.Error, e.StackTrace);
            }
        }

    }
}
