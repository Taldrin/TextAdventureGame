using InterfurCreations.AdventureGames.Core.Objects;
using InterfurCreations.AdventureGames.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InterfurCreations.AdventureGames.Core.Services
{
    public static class ErrorMessageHelper
    {
        public static string MakeMessage(Exception e, PlayerGameSave gameSave, string additional = "")
        {
            return MakeMessage(e, new PlayerState { player = new Player { ActiveGameSave = gameSave } }, additional);
        }

        public static string MakeMessage(Exception e, string additional = "")
        {
            return MakeMessage(e, gameState: null, additional);
        }

        public static string MakeMessage(Exception e, PlayerState gameState, string additional = "")
        {
            var messages = string.Join("\n", e.GetInnerExceptions().Select(a => a.Message));
            var stackTrace = e.StackTrace;

            string player = "NOPLAYER";
            string saveData = "NODATA";
            string gameName = "NOGAME";
            string stateId = "NA";

            if(gameState != null && gameState.player != null)
            {
                player = gameState.player.Name;
                if(gameState.player.ActiveGameSave != null)
                {
                    gameName = gameState.player.ActiveGameSave.GameName;
                    if(gameState.player.ActiveGameSave.GameSaveData != null)
                    {
                        saveData = string.Join(" ", gameState.player.ActiveGameSave.GameSaveData.Select(a => $"[N: {a.Name} V: {a.Value}] "));
                    }
                    stateId = gameState.player.ActiveGameSave.StateId;
                }
            }

            return $"ERROR: {additional} \n\nPlayer: {player}\nGame: {gameName} \nState ID: {stateId} \nException messages: \n{messages} \n\nSave data: \n{saveData} \n\nStack trace\n{stackTrace}";
        }
    }
}
