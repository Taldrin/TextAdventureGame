﻿using InterfurCreations.AdminSite.Core.DataObjects;
using InterfurCreations.AdminSite.Core.Interfaces;
using InterfurCreations.AdventureGames.Database;
using InterfurCreations.AdventureGames.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InterfurCreations.AdminSite.Core
{
    public class ActionResolver : IActionResolver
    {
        private readonly IGameStore _gameStore;
        public ActionResolver(IGameStore gameStore)
        {
            _gameStore = gameStore;
        }

        public ActionDetailsDataObject ResolveAction(PlayerAction action, TimeSpan? liveGameTimeBetweenCheck = null)
        {
            List<DrawGame> games;
            if(liveGameTimeBetweenCheck == null)
                games = _gameStore.ListGames();
            else
                games = _gameStore.ListGames(liveGameTimeBetweenCheck.Value);

            var actionGame = games.SingleOrDefault(a => a.GameName == action.GameName);

            var option = actionGame.FindOptionById(action.ActionName);

            return new ActionDetailsDataObject
            {
                StateOptionTaken = option.StateText,
                CurrentStateText = option.ResultState.StateText
            };
        }
    }
}
