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
using System.Net.Sockets;
using Discord.Interactions;
using InterfurCreations.AdventureGames.Core.DataObjects;
using System.Linq;

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

            socketClient.Disconnected += SocketClient_Disconnected;
            await socketClient.SetGameAsync("ft.start", null, ActivityType.Playing);

            socketClient.MessageReceived += MessageExecutor;
            socketClient.ButtonExecuted += ButtonExecuted;
            socketClient.JoinedGuild += SocketClient;
        }

        private async Task SocketClient(SocketGuild arg)
        {
            var validChannel = arg.TextChannels.FirstOrDefault(a => a.IsNsfw);
            if (validChannel == null)
            {
                var channel = arg.TextChannels.FirstOrDefault();
                await channel.SendMessageAsync("Uh oh! No NSFW channel found! Message the bot directly to interact! Or create a NSFW channel and @ the bot to start!");
            } else
            {
                var builder = new ComponentBuilder();
                builder = builder.WithButton("Start", "Start");
                await validChannel.SendMessageAsync("Click to begin here, message the bot directly, or '@' the bot in another channel!", components: builder.Build());
            }
        }

        private async Task SocketClient_Disconnected(Exception arg)
        {
            try
            {
                await socketClient.StopAsync();
                await socketClient.LogoutAsync();

                socketClient.MessageReceived -= MessageExecutor;
                socketClient.Disconnected -= SocketClient_Disconnected;
            }
            catch (Exception e)
            {
                _reporter.ReportError("Error trying to logout: " + e.Message);
            }
            finally
            {
                SetupAsync();
            }
        }

        private Task SocketClient_Log(LogMessage arg)
        {
            return Task.Run(() => Log.LogMessage(arg.Message, LogType.General));
        }

        private async Task MessageExecutor(SocketMessage arg)
        {
            bool isMentioned = false;
            if (arg.MentionedUsers.Any(a => a.Id == socketClient.CurrentUser.Id))
            {
                isMentioned = true;
            }
            if (arg.Author.IsBot) return;
            // Filter out any messages that don't start with 'ft.'
            if (!(arg.Channel is SocketDMChannel) && !isMentioned) return;

            if (arg.Channel is ITextChannel textChannel && !(arg.Channel is SocketDMChannel) && !textChannel.IsNsfw)
            {
                await arg.Channel.SendMessageAsync("OwO? This bot contains adult content, " +
                    "and so must be used either in a NSFW channel, or privately messaged!");
                return;
            }

            var msg = arg.Content;
            if(isMentioned)
                msg = arg.Content.Split(">")[1].Trim();

            await MessageFeedAsync(msg, arg.Channel, (long)arg.Author.Id, arg.Author.Username);
        }

        private async Task MessageFeedAsync(string message, ISocketMessageChannel channel, long authorId, string playerName, SocketMessageComponent messageComp = null)
        {
            try
            {
                using (var scope = ContainerStore.Container.BeginLifetimeScope())
                {
                    var discordMessageHandler = scope.Resolve<DiscordMessageHandler>();
                    var result = await discordMessageHandler.MessageReceived(message, authorId, playerName);

                    for (int i = 0; i < result.MessagesToShow.Count - 1; i++)
                    {
                        await channel.SendMessageAsync(result.MessagesToShow[i].Message);
                    }
                    var builder = new ComponentBuilder();
                    foreach (var option in result.OptionsToShow)
                    {
                        var displayedOption = option;
                        if (option.Length > 80)
                            displayedOption = option.Substring(0, 76) + "...";
                        builder.WithButton(displayedOption, option);
                    }
                    var msgToSend = result.MessagesToShow.Last().Message;
                    if(string.IsNullOrWhiteSpace(msgToSend))
                    {
                        Log.LogMessage("No messages to send error. " + result.StatesVisited?.LastOrDefault(), LogType.Error);
                        msgToSend = "Encountered an error - empty text response. If you're in a stuck state, try typing in '-Menu-' via DMs, or '@' the bot with '-Menu-'. This error has been automatically reported.";
                    }
                    await channel.SendMessageAsync(msgToSend, components: builder.Build());
                }
            }
            catch (Exception e)
            {
                Log.LogMessage(e.Message, LogType.Error, e.StackTrace);
            }
        }

        private async Task ButtonExecuted(SocketMessageComponent arg)
        {
            if (arg.Channel is ITextChannel textChannel && !(arg.Channel is SocketDMChannel) && !textChannel.IsNsfw)
            {
                await arg.Channel.SendMessageAsync("OwO? This bot contains adult content, " +
                    "and so must be used either in a NSFW channel, or privately messaged!");
                return;
            }
            await arg.DeferAsync();
            await MessageFeedAsync(arg.Data.CustomId, arg.Channel, (long)arg.User.Id, arg.User.Username, arg);
        }
    }
}
