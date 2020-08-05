using Discord.WebSocket;
using InterfurCreations.AdventureGames.Core.DataObjects;
using InterfurCreations.AdventureGames.Core.Interface;
using InterfurCreations.AdventureGames.Core.Objects;
using InterfurCreations.AdventureGames.Database;
using InterfurCreations.AdventureGames.DatabaseServices.Interfaces;
using InterfurCreations.AdventureGames.Logging;
using InterfurCreations.AdventureGames.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Discord
{
    public class DiscordMessageHandler
    {
        private readonly IPlayerDatabaseController _playerDatabaseController;
        private readonly IGameExecutor _gameExecutor;
        private readonly IAccountController _accountController;
        private readonly IReporter _reporter;

        public DiscordMessageHandler(IPlayerDatabaseController playerDatabaseController, IGameExecutor gameExecutor, IAccountController accountController, IReporter reporter)
        {
            _playerDatabaseController = playerDatabaseController;
            _gameExecutor = gameExecutor;
            _accountController = accountController;
            _reporter = reporter;
        }

        public async Task MessageReceived(SocketMessage arg)
        {
            string message = null;

            if (!(arg.Channel is SocketDMChannel)) {
                if (!arg.Content.StartsWith("ft.")) return;
                    message = arg.Content.Remove(0, 3);
            } else
            {
                message = arg.Content;
                if (message.StartsWith("ft."))
                {
                    message = arg.Content.Remove(0, 3);
                }
            }

            var playerName = arg.Author.Username;
            var channelId = (long)arg.Channel.Id;
            if (arg.Author.IsBot) return;

            var playerState = _playerDatabaseController.GetPlayerByDiscordAuthor((long)arg.Author.Id);
            PlayerState dtoPlayer;
            if (playerState != null)
            {
                dtoPlayer = new PlayerState { player = playerState };
            }
            else
            {
                var newPlayer = await CreateNewGame(arg.Channel, playerName, (long)arg.Author.Id);
                dtoPlayer = new PlayerState { player = newPlayer };
            }

            message = TryGetMessageChosen(arg.Channel, dtoPlayer, message);

            if (string.IsNullOrEmpty(message)) message = "NULL_MESSAGE";

            ExecutionResult result = _gameExecutor.ProcessNewMessage(message, dtoPlayer);

            if (result != null)
            {
                if (result.IsInvalidInput)
                    await SendMessageAsync(arg.Channel, "**Invalid Input!**");
                foreach (var resultMessage in result.MessagesToShow)
                {
                    await SendMessageAsync(arg.Channel, resultMessage.Message);
                }

                await SendOptionMessagesAsync(arg.Channel, result.OptionsToShow);
            }
        }

        private string TryGetMessageChosen(ISocketMessageChannel channel, PlayerState player, string message)
        {
            var formerOptions = _gameExecutor.GetPossibleOptionsFromState(player);
            if (!int.TryParse(message, out var intMessage))
            {
                return message;
            }

            if (formerOptions.Count < intMessage || intMessage <= 0)
            {
                return null;
            }

            return formerOptions[intMessage - 1];
        }

        private async Task<Player> CreateNewGame(ISocketMessageChannel channel, string name, long playerId)
        {
            var player = _accountController.GetOrCreateNewDiscordAccount(playerId, name);
            _reporter.ReportMessage("The bot was started in a new chat, with username: " + name);
            await ShowDefaultMessage(channel);
            return player;
        }

        public async Task SendMessageAsync(ISocketMessageChannel channel, string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;
            if (message.Length > 2000)
            {
                var splitMessage = message;
                while (splitMessage.Length > 2000)
                {
                    var splitIndex = splitMessage.Substring(0, 2000).LastIndexOf(' ');
                    var toSend = splitMessage.Substring(0, splitIndex);
                    await channel.SendMessageAsync(toSend);
                    splitMessage = splitMessage.Substring(splitIndex, splitMessage.Length - splitIndex);
                }
                await channel.SendMessageAsync(splitMessage);
            } else
            {
                await channel.SendMessageAsync(message);
            }
        }

        public async Task SendOptionMessagesAsync(ISocketMessageChannel channel, List<string> replies)
        {
            string repliesString = "";
            int index = 1;
            replies.ForEach(a =>
            {
                if(channel is SocketDMChannel)
                    repliesString = repliesString + "**" + index++ + "** - " + a + "\n";
                else
                    repliesString = repliesString + "**ft." + index++ + "** - " + a + "\n";
            });
            repliesString.TrimEnd('n', '\\');
            if (string.IsNullOrEmpty(repliesString)) return;
            await channel.SendMessageAsync(repliesString);
        }

        private async Task ShowDefaultMessage(ISocketMessageChannel channel)
        {
            await channel.SendMessageAsync("To use this disocrd bot, reply with the 'ft.' plus the number of the option you wish to take! For example, " +
                                    "it may say \nft.1: Run Away\nft.2: Stand and fight\nIf you wished to Stand and fight, simply reply to the bot with" +
                                    " the message 'ft.2'.");
        }

    }
}

