using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.GameTesting
{
    public interface IGameTestDataProvider
    {
        GameTestingGameData ProvideData(string gameName);
        GameTestDataStore ProvideDataForGameTesting(string gameName);
        void SaveDataForGame(GameTestDataStore dataStore, string gameName);
        void DeleteDataForGame(string gameName);
    }
}
