using InterfurCreations.AdventureGames.Database;
using InterfurCreations.AdventureGames.Graph;
using InterfurCreations.AdventureGames.Graph.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InterfurCreations.AdventureGames.GraphServices
{
    public class ActionResolver
    {
        private readonly IGameStore _gameStore;
        public ActionResolver(IGameStore gameStore)
        {
            _gameStore = gameStore;
        }

        public ActionDetailsDataObject ResolveAction(PlayerAction action)
        {
            List<DrawGame> games;
            games = _gameStore.ListGames();

            var actionGame = games.SingleOrDefault(a => a.GameName == action.GameName);

            var option = actionGame.FindOptionById(action.ActionName);

            return new ActionDetailsDataObject
            {
                StateOptionTaken = option.StateText,
                StateText = option.ResultState.StateText
            };
        }
    }
}
