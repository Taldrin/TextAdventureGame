using InterfurCreations.AdventureGames.Graph;
using InterfurCreations.AdventureGames.Graph.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Furventure.AdventureGames.Offline.Core
{
    public class GameFileDrawStore : IGameStore
    {
        public List<string> CheckForOutOfDateGames(Dictionary<DrawGame, DateTime> timeRetrievedGames)
        {
            var files = getFiles();
            var outOfDateGames = timeRetrievedGames.Where(a => a.Value < files.FirstOrDefault(b => b.Name == a.Key.GameName)?.LastWriteTime);
            var newGames = files.Where(a => !timeRetrievedGames.Keys.Any(b => b.GameName == a.Name)).GroupBy(a => a.Name).Select(a => a.First()).ToList();
            return outOfDateGames.ToList().Select(a => a.Key.GameName).ToList().Concat(newGames.Select(a => a.Name)).ToList();
        }

        public byte[] GetGame(string game)
        {
            var foundGame = getFiles().Where(a => a.Name == game).OrderByDescending(a => a.LastWriteTime).FirstOrDefault();
            var gameBytes = File.ReadAllBytes(foundGame.FullName);
            return gameBytes;
        }

        private List<FileInfo> getFiles()
        {
            var dir = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Games"));
            return dir.GetFiles().ToList();
        }
    }
}
