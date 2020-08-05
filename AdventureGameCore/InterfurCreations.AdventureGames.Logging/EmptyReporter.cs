using System;
using System.Collections.Generic;
using System.Text;

namespace InterfurCreations.AdventureGames.Logging
{
    public class EmptyReporter : IReporter
    {
        public void Initialise()
        {
        }

        public void ReportError(string error)
        {
        }

        public void ReportMessage(string message)
        {
        }

        public bool UserReportMessage(string message)
        {
            return true;
        }
    }
}
