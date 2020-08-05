using InterfurCreations.AdventureGames.Core.Interface;
using InterfurCreations.AdventureGames.Core.Objects;
using InterfurCreations.AdventureGames.Database;
using InterfurCreations.AdventureGames.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InterfurCreations.AdventureGames.GameTesting
{
    public class GameTestExecutor
    {
        private IGameProcessor _gameProcessor;

        public GameTestExecutor(IGameProcessor gameProcessor)
        {
            _gameProcessor = gameProcessor;
        }

        public ExecutionResult ExecuteAction(string option, Player player, DrawGame game)
        {
            try
            {
                var execResult = _gameProcessor.ProcessMessage(option, player.ActiveGameSave, game, player);

                return new ExecutionResult
                {
                    MessagesReceived = execResult.MessagesToShow,
                    NewState = new PlayerState { player = player },
                    OptionsReceived = execResult.OptionsToShow,
                };
            }
            catch (Exception e)
            {
                return new ExecutionResult
                {
                    ErrorsReceived = new List<Exception> { e },
                    IsErrored = true
                };
            }
        }
    }
}
