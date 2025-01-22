using InterfurCreations.AdventureGames.Logging;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLMFineTuningDataGenerator
{
    public class ConsoleReporter : IReporter
    {
        public void Initialise()
        {
        }

        public void ReportError(string error)
        {
            AnsiConsole.Markup($"\n[red]{error}[/]");
        }

        public void ReportMessage(string message)
        {
            AnsiConsole.Markup($"\n{message}");
        }

        public bool UserReportMessage(string message)
        {
            AnsiConsole.Markup($"\n{message}");
            return true;
        }
    }
}
