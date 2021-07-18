using Furventure.AdventureGames.DatabaseServices.Offline;
using InterfurCreations.AdventureGames.Core.DataObjects;
using InterfurCreations.AdventureGames.Core.Interface;
using InterfurCreations.AdventureGames.Core.Objects;
using System;

namespace Furventure.AdventureGames.Offline.Core
{
    public class OfflineMessageProcessor
    {
        private readonly IGameExecutor _gameExecutor;
        private readonly IOfflinePlayerController _playerController;

        public OfflineMessageProcessor(IGameExecutor gameExecutor, IOfflinePlayerController playerController)
        {
            _gameExecutor = gameExecutor;
            _playerController = playerController;
        }

        public ExecutionResult ProcessMessage(string message)
        {
            var player = _playerController.GetPlayerByProfile("default");
            return _gameExecutor.ProcessNewMessage(message, new PlayerState { player = player });
        }
    }
}
