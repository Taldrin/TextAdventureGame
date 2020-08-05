using InterfurCreations.AdventureGames.Database;
using InterfurCreations.AdventureGames.Graph;
using System.Collections.Generic;

namespace InterfurCreations.AdventureGames.Core.Interface
{
    public interface IGameProcessor
    {
        System.Collections.Generic.List<string> GetCurrentOptions(PlayerGameSave playerGameData, Graph.DrawGame game, DrawState currentDrawGameState = null);
        List<(string option, StateOption optionData)> GetCurrentOptionsFullDrawData(PlayerGameSave playerGameData, DrawGame game, DrawState currentDrawGameState = null);
        DataObjects.ExecutionResult ProcessMessage(string message, PlayerGameSave playerGameData, Graph.DrawGame game, Player player);
        (System.Collections.Generic.List<DataObjects.MessageResult> Messages, DrawState EndingState, List<string> StatesVisited) RecursivelyHandleStates(DrawState currentState, PlayerGameSave gameSave, Player player, bool withDataChanges = true);
    }
}