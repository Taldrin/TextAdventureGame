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
            var dayBehind = DateTime.Now.Subtract(TimeSpan.FromDays(1));
            var playerActionsToday = _databaseProvider.GetContext().PlayerActions.Where(a => a.Time > dayBehind).Include(a => a.Player);
            _reporter.ReportMessage("#### This is your daily usage report! ####");
            _reporter.ReportMessage("There were a total of: " + playerActionsToday.Count() + " actions performed today");

            var playerActionsTodayResolved = playerActionsToday.ToList();
            _reporter.ReportMessage("#### Games ####");
            foreach (var group in playerActionsTodayResolved.GroupBy(a => a.GameName))
            {
                _reporter.ReportMessage(group.Count() + " of those actions were performed on the game: " + group.First().GameName);
            }

            _reporter.ReportMessage("#### Players ####");
            playerActionsTodayResolved.GroupBy(a => a.PlayerId).ToList().ForEach(a =>
            {
                _reporter.ReportMessage(a.Count() + " of those actions were performed by the player: " + a.First().Player.Name);
            });
            _reporter.ReportMessage("#### End of report! :) ####");

        } 
    }
}
