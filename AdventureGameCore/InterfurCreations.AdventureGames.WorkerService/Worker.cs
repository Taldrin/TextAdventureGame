using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using InterfurCreations.AdventureGames.BotMain;
using InterfurCreations.AdventureGames.Logging;
using InterfurCreations.AdventureGames.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace InterfurCreations.AdventureGames.WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Log.LogMessage("Bot service started and running. Calling Main method.");
            ProgramRun.Run();
        }
    }
}
