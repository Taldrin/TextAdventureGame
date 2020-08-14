using System.Collections.Generic;
using InterfurCreations.AdventureGames.Core.DataObjects;
using InterfurCreations.AdventureGames.Core.Interface;
using InterfurCreations.AdventureGames.Core.Objects;
using InterfurCreations.AdventureGames.Database;
using InterfurCreations.AdventureGames.DatabaseServices.Interfaces;
using InterfurCreations.AdventureGames.Logging;
using InterfurCreations.AdventureGames.Services;
using InterfurCreations.AdventureGames.Telegram.DataObjects;

namespace InterfurCreations.AdventureGames.Telegram
{
    public class TelegramBotOverseer
    {
        private IReporter _reporter;
        private IGameExecutor _gameExecutor;
        private IAccountController _accountController;
        private IPlayerDatabaseController _playerDatabaseController;

        public TelegramBotOverseer(IGameExecutor gameExecutor, IReporter reporter, IPlayerDatabaseController playerDatabaseController, IAccountController accountController)
        {
            _reporter = reporter;
            _gameExecutor = gameExecutor;
            _accountController = accountController;
            _playerDatabaseController = playerDatabaseController;
        }

        public async void RecieveNewMessage(Message message, TelegramService service)
        {
            if (message == null || message.text == null) return;
            Log.LogMessage("Recivied new message: " + message.text, LogType.General);

            string chatName = message.chat.username;
            if (string.IsNullOrWhiteSpace(chatName))
            {
                chatName = message.chat.first_name;
            }

            var playerState = _playerDatabaseController.GetPlayerByTelegramChannel(message.chat.id);

            if (playerState == null) {
                Log.LogMessage("Instance was not found for id: " + message.chat.id + " Creating new game instance", LogType.General);
                _reporter.ReportMessage("The bot was started in a new chat, with username: " + chatName);
                playerState = CreateNewGame(message.chat.id, chatName);
            }
            else
            {
                Log.LogMessage("Existing instance found for chat: " + message.chat.id + " Sending message", LogType.General);
            }
            var executionResult = _gameExecutor.ProcessNewMessage(message.text, new PlayerState { player = playerState });

            if (executionResult != null)
            {
                foreach (var a in executionResult.MessagesToShow)
                {
                    if(!string.IsNullOrWhiteSpace(a.ImageUrl))
                    {
                        await Methods.sendImageWithReplyOptions(service, a.ImageUrl, message.chat.id, executionResult.OptionsToShow);
                    }
                    else if(!string.IsNullOrWhiteSpace(a.Message))
                        await Methods.sendMessageWithReplyOptions(service, a.Message, message.chat.id, executionResult.OptionsToShow);
                };
                if (executionResult.IsInvalidInput)
                    await Methods.sendMessage(service, "That's not a valid input!", message.chat.id);
            }
        }

        public Player CreateNewGame(int chatId, string name)
        {
            return _accountController.GetOrCreateNewTelegramAccount(chatId, name);
        }
    }
}
