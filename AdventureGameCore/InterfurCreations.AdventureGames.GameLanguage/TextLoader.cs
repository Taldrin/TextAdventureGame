using InterfurCreations.AdventureGames.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.GameLanguage
{
    public class TextLoader
    {
        public List<string> LoadText(string fileName)
        {
            try
            {
                string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Text\" + fileName + ".txt");
                var lines = File.ReadAllLines(path).ToList();
                return lines;
            } catch (Exception e)
            {
                Log.LogMessage("Error loading text: " + fileName + " " + e.Message, LogType.Error);
            }
            return new List<string>();
        }
    }
}
