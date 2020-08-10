using InterfurCreations.AdventureGames.Logging;
using InterfurCreations.AdventureGames.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace InterfurCreations.AdminSite.Core
{
    public class EmptyReporterService : IReporter
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
            return false;
        }
    }
}
