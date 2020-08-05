using InterfurCreations.AdventureGames.Database;
using InterfurCreations.AdventureGames.Graph;
using InterfurCreations.AdventureGames.Graph.Store;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InterfurCreations.AdventureGames.GraphServices
{
    public class PlayerGraphGenerator
    {
        private readonly Player _player;
        private readonly IGameStore _drawStore;

        public PlayerGraphGenerator(Player player, IGameStore drawStore)
        {
            _player = player;
            _drawStore = drawStore;
        }

        public DrawState GenerateLastGame()
        {
            var actionResolver = new ActionResolver(_drawStore);

            var mostRecentGame = _player.Actions.OrderByDescending(a => a.Time).GroupBy(a => a.GameName).FirstOrDefault();
            var gameActions = mostRecentGame.ToList();

            var games = _drawStore.ListGames();

            var game = games.FirstOrDefault(a => a.GameName.ToLower() == gameActions.First().GameName.ToLower());

            var firstAction = gameActions.FirstOrDefault();
            var firstOption = game.FindOptionById(firstAction.ActionName);

            DrawState currentState = new DrawState
            {
                Id = firstOption.ResultState.Id,
                StateText = firstOption.ResultState.StateText,
            };
            var firstState = currentState;

            gameActions = gameActions.Skip(1).ToList();
            foreach(var action in gameActions)
            {
                var option = game.FindOptionById(action.ActionName);

                if (option == null) continue;

                var newState = new DrawState
                {
                    Id = option.ResultState.Id,
                    StateText = option.ResultState.StateText,
                };
                currentState.StateOptions = new List<StateOption>
                {
                    new StateOption
                    {
                        Id = option.Id,
                        StateText = option.StateText,
                        ResultState = newState,
                    }
                };

                currentState = newState;
            }

            return firstState;

        }
    }
}
