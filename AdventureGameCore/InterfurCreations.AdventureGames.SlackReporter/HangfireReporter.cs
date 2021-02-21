using Hangfire;
using InterfurCreations.AdventureGames.DatabaseServices.Interfaces;
using InterfurCreations.AdventureGames.Logging;

namespace InterfurCreations.AdventureGames.SlackReporter
{
    public class HangfireReporter
    {
        public HangfireReporter()
        {

        }

        public void SetupJobs(IDatabaseContextProvider databaseProvider, IReporter reporter)
        {
            RecurringJob.RemoveIfExists("DailyReport");
            RecurringJob.RemoveIfExists("WeeklyReport");

            RecurringJob.AddOrUpdate<ISlackReportGenerator>("DailyReport", a => a.DailyUsageReport(), Cron.Daily);
        }
    }
}
