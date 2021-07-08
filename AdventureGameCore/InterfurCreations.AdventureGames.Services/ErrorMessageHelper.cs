using InterfurCreations.AdventureGames.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InterfurCreations.AdventureGames.Core.Services
{
    public static class ErrorMessageHelper
    {

        public static string MakeMessage(Exception e, string additional = "")
        {
            return MakeMessage(e, gameState: null, additional);
        }

        public static string MakeMessage(Exception e, PlayerGameSave gameState, string additional = "")
        {
            var messages = e.GetInnerExceptions().Select(a => a.Message);
            var stackTrace = e.StackTrace;

            string saveData = "NODATA";
            string gameName = "NOGAME";
            string player = "NOPLAYER";
            gameName = gameState?.GameName;
            if (gameState?.GameSaveData != null)
            {
                saveData = string.Join(" ", gameState.GameSaveData.Select(a => $"[N: {a?.Name} V: {a?.Value}] "));
            }

            return $"ERROR: {additional} \n\nPlayer: {player}\nGame: {gameName} \nException messages: \n{messages} \n\nSave data: \n{saveData}\n\nStack trace\n{stackTrace}";
        }

        public static IEnumerable<Exception> GetInnerExceptions(this Exception ex)
        {
            if (ex == null)
            {
                throw new ArgumentNullException("ex");
            }

            var innerException = ex;
            do
            {
                yield return innerException;
                innerException = innerException.InnerException;
            }
            while (innerException != null);
        }
    }
}
