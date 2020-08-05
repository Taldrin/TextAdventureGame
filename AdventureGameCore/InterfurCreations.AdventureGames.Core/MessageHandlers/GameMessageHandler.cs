using InterfurCreations.AdventureGames.Core.DataObjects;
using InterfurCreations.AdventureGames.Core.Interface;
using InterfurCreations.AdventureGames.Database;
using InterfurCreations.AdventureGames.Exceptions;
using InterfurCreations.AdventureGames.Graph;
using InterfurCreations.AdventureGames.Graph.Store;
using System.Collections.Generic;
using System.Linq;

namespace InterfurCreations.AdventureGames.Core.MessageHandlers
{
    public class GameMessageHandler : IMessageHandler
    {
        private readonly IGameStore _gameStore;
        private readonly IGameProcessor _gameProcessor;
        
        public GameMessageHandler(IGameStore gameStore, IGameProcessor gameProcessor)
        {
            _gameStore = gameStore;
            _gameProcessor = gameProcessor;
        }

        public int Priorioty => 1;

        public List<string> GetOptions(Player player)
        {
            var gameSave = player.ActiveGameSave;
            var gameBeingPlayed = FindGame(player, gameSave);
            var options = _gameProcessor.GetCurrentOptions(gameSave, gameBeingPlayed);
            if (options.Count == 0)
            {
                options.Add(Messages.Restart);
                options.Add(Messages.MainMenu);
            }
            else
            {
                options.Add("-Menu-");
            }
            return options;
        }

        public ExecutionResult HandleMessage(string message, Player player)
       {
            var gameSave = player.ActiveGameSave;
            var gameBeingPlayed = FindGame(player, gameSave);

            if(message == Messages.Restart)
            {
                var execResult = GameService.LaunchGameForPlayer(gameBeingPlayed, player.ActiveGameSave, player, _gameProcessor);
                execResult.OptionsToShow.Add("-Menu-");
                return execResult;
            }
            if(message == Messages.MainMenu)
            {
                return MessageHandlerHelpers.ReturnToMainMenu(player);
            }

            var messageResult = _gameProcessor.ProcessMessage(message, gameSave, gameBeingPlayed, player);
            if (messageResult.OptionsToShow.Count == 0)
            {
                messageResult.OptionsToShow.Add(Messages.Restart);
                messageResult.OptionsToShow.Add(Messages.MainMenu);
            }
            else
            {
                messageResult.OptionsToShow.Add("-Menu-");
            }
            return messageResult;
        }

        public DrawGame FindGame(Player player, PlayerGameSave gameSave)
        {
            var games = _gameStore.ListGames();
            var gamesFound = games.Where(a => a.GameName == gameSave.GameName).ToList();

            if (gamesFound.Count == 0)
                throw new AdventureGameException("Could not find the game being played for user: " + player.Name + " and game name: " + gameSave.GameName + ". It may have been removed - sending back to main menu.", true);
            if (gamesFound.Count > 1)
                throw new AdventureGameException("Found more than one game applicable for player: " + player.Name + " and game name: " + gameSave.GameName + ". Something has therefore critically wrong.", true);

            var gameBeingPlayed = gamesFound.First();

            return gameBeingPlayed;
        }

        public bool ShouldHandleMessage(string message, string gameState, string playerFlag)
        {
            return gameState != null && playerFlag == PlayerFlag.IN_GAME.ToString();

        }
    }
}
