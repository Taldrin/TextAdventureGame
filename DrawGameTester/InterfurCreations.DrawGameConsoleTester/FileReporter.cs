using InterfurCreations.AdventureGames.Logging;
using InterfurCreations.AdventureGames.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace InterfurCreations.DrawGameConsoleTester
{
    public class FileReporter : IReporter
    {
        private string filePath;

        public void Initialise()
        {
            filePath = Path.Combine(Directory.GetCurrentDirectory(), "GameTesterOutput.txt");
            File.Create(filePath);
        }

        public void ReportError(string error)
        {
            File.AppendAllText(filePath, error);
            File.AppendAllText(filePath, "\n");
        }

        public void ReportMessage(string message)
        {
            File.AppendAllText(filePath, message);
            File.AppendAllText(filePath, "\n");
        }

        public bool UserReportMessage(string message)
        {
            File.AppendAllText(filePath, message);
            File.AppendAllText(filePath, "\n");
            return true;
        }
    }
}
