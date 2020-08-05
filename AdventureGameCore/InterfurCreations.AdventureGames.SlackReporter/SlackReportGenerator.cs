using InterfurCreations.AdventureGames.DatabaseServices.Interfaces;
using Microsoft.EntityFrameworkCore;
using InterfurCreations.AdventureGames.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfurCreations.AdventureGames.Logging;

namespace InterfurCreations.AdventureGames.SlackReporter
{
    public class SlackReportGenerator : ISlackReportGenerator
    {

        private IDatabaseContextProvider _databaseProvider;
        private IReporter _reporter;

        public SlackReportGenerator(IDatabaseContextProvider databaseProvider, IReporter reporter)
        {
            _databaseProvider = databaseProvider;
            _reporter = reporter;
        }

        public void DailyUsageReport()
        {
            var playerActionsToday = _databaseProvider.GetContext().PlayerActions.Where(a => a.Time > DateTime.Now.Subtract(TimeSpan.FromDays(1))).Include(a => a.Player);
            _reporter.ReportMessage("#### This is your daily usage report! ####");
            _reporter.ReportMessage("There were a total of: " + playerActionsToday.Count() + " actions performed today");

            _reporter.ReportMessage("#### Games ####");
            playerActionsToday.GroupBy(a => a.GameName).ToList().ForEach(a =>
            {
                _reporter.ReportMessage(a.Count() + " of those actions were performed on the game: " + a.First().GameName);
            });

            _reporter.ReportMessage("#### Players ####");
            playerActionsToday.GroupBy(a => a.PlayerId).ToList().ForEach(a =>
            {
                _reporter.ReportMessage(a.Count() + " of those actions were performed by the player: " + a.First().Player.Name);
            });
            _reporter.ReportMessage("#### End of report! :) ####");

        } 
    }
}
