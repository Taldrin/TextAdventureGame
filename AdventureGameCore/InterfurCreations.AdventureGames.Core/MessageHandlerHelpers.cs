using InterfurCreations.AdventureGames.Core.DataObjects;
using InterfurCreations.AdventureGames.Core.Interface;
using InterfurCreations.AdventureGames.Core.MessageHandlers;
using InterfurCreations.AdventureGames.Database;
using InterfurCreations.AdventureGames.GameLanguage;
using InterfurCreations.AdventureGames.Graph;
using InterfurCreations.AdventureGames.Graph.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Core
{
    public static class MessageHandlerHelpers
    {
        private static IEnumerable<IMessageHandler> _messageHandlers;

        public static void SetHandlers(IEnumerable<IMessageHandler> messageHandlers)
        {
            _messageHandlers = messageHandlers;
        }

        public static ExecutionResult ReturnToMainMenu(Player player)
        {
            player.PlayerFlag = PlayerFlag.MAIN_MENU.ToString();
            var mainMenuHandler = _messageHandlers.SingleOrDefault(a => a is MainMenuMessageHandler);
            return mainMenuHandler.HandleMessage("", player);
        }

        public static ExecutionResult ReturnToGame(Player player, IGameRetrieverService _gameStoreService, ITextParsing textParsing, IGameProcessor gameProcessor)
        {
            player.PlayerFlag = PlayerFlag.IN_GAME.ToString();
            var games = _gameStoreService.ListGames();
            var playerState = player.ActiveGameSave;
            var gameFound = games.Where(a => a.GameName == playerState.GameName).FirstOrDefault();
            var state = gameFound.FindStateById(playerState.StateId);
            var execResult = new ExecutionResult
            {
                MessagesToShow = new List<MessageResult> { new MessageResult { Message = textParsing.ParseText(playerState, state.StateText) } },
                OptionsToShow = gameProcessor.GetCurrentOptions(playerState, gameFound, state)
            };
            execResult.OptionsToShow.Add("-Menu-");
            return execResult;
        }

        public static ExecutionResult ReturnToGameMenu(Player player, string message)
        {
            var menuHandler = _messageHandlers.SingleOrDefault(a => a is MenuMessageHandler);
            return menuHandler.HandleMessage(message, player);

        }
    }
}
