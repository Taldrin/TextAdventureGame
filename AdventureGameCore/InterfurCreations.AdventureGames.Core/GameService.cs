using InterfurCreations.AdventureGames.Core.DataObjects;
using InterfurCreations.AdventureGames.Core.Interface;
using InterfurCreations.AdventureGames.Database;
using InterfurCreations.AdventureGames.Graph;
using System;
using System.Collections.Generic;
using System.Text;

namespace InterfurCreations.AdventureGames.Core
{
    public static class GameService
    {
        public static ExecutionResult LaunchGameForPlayer(DrawGame game, PlayerGameSave save, Player player, IGameProcessor gameProcessor)
        {
            player.ActiveGameSave = new PlayerGameSave
            {
                GameName = game.GameName,
                StateId = game.StartState.Id,
            };

            var messages = gameProcessor.RecursivelyHandleStates(game.StartState, player.ActiveGameSave, player, game, true);
            messages.Messages.Reverse();

            player.ActiveGameSave = new PlayerGameSave
            {
                GameName = game.GameName,
                StateId = messages.EndingState.Id,
            };

            player.PlayerFlag = PlayerFlag.IN_GAME.ToString();
            var execResult = new ExecutionResult { MessagesToShow = messages.Item1, OptionsToShow = gameProcessor.GetCurrentOptions(player.ActiveGameSave, game) };
            return execResult;
        }
    }
}
