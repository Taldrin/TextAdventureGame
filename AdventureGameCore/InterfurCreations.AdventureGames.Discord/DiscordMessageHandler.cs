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

        public async Task<ExecutionResult> MessageReceived(string message, long authorId, string playerName)
        {

            var playerState = _playerDatabaseController.GetPlayerByDiscordAuthor(authorId);
            PlayerState dtoPlayer;
            if (playerState != null)
            {
                dtoPlayer = new PlayerState { player = playerState };
            }
            else
            {
                var newPlayer = await CreateNewGame(playerName, authorId);
                dtoPlayer = new PlayerState { player = newPlayer };
            }

            if (string.IsNullOrEmpty(message)) message = "NULL_MESSAGE";

            ExecutionResult result = _gameExecutor.ProcessNewMessage(message, dtoPlayer);

            if (result != null)
            {

                // Combine all the messages into one, then split them up into max 2000 characters.
                // This is for performance, as sending multiple messages to Discord is sloooow
                // This way, we send as few messages as possible
                var combinedText = string.Join("\n\n", result.MessagesToShow.Select(a => string.IsNullOrEmpty(a.ImageUrl) ? a.Message : a.ImageUrl).ToList());
                result.MessagesToShow = new List<MessageResult> { new MessageResult { Message = combinedText } };

                HandleLongMessages(result);

                return result;
            } else
            {
                return new ExecutionResult
                {
                    MessagesToShow = new List<MessageResult> { new MessageResult { Message = "**Invalid Input**" } }
                };
            }
        }

        private async Task<Player> CreateNewGame(string name, long playerId)
        {
            var player = _accountController.GetOrCreateNewDiscordAccount(playerId, name);
            _reporter.ReportMessage("The bot was started in a new chat, with username: " + name);
            return player;
        }

        public void HandleLongMessages(ExecutionResult result)
        {
            for(int i = result.MessagesToShow.Count() - 1; i >= 0; i--)
            {
                var message = result.MessagesToShow[i];
                if(message.Message.Length > 2000)
                {
                    result.MessagesToShow.RemoveAt(i);
                    var newMessages = SplitMessage(message.Message);
                    foreach(var newMessage in newMessages)
                    {
                        result.MessagesToShow.Insert(i, new MessageResult { Message = newMessage, ImageUrl = message.ImageUrl });
                    }
                }
            }
        }

        public List<string> SplitMessage(string message)
        {
            List<string> messages = new List<string>();
            if (string.IsNullOrWhiteSpace(message)) return messages;
            if (message.Length > 2000)
            {
                var splitMessage = message;
                while (splitMessage.Length > 2000)
                {
                    var splitIndex = splitMessage.Substring(0, 2000).LastIndexOf(' ');
                    var toSend = splitMessage.Substring(0, splitIndex);
                    messages.Add(toSend);
                    splitMessage = splitMessage.Substring(splitIndex, splitMessage.Length - splitIndex);
                }
                messages.Add(splitMessage);
            }
            else
            {
                messages.Add(message);
            }
            return messages;
        }
    }
}

