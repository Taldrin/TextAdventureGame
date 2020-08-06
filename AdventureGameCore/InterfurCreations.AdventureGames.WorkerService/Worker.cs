using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using InterfurCreations.AdventureGames.BotMain;
using InterfurCreations.AdventureGames.Logging;
using InterfurCreations.AdventureGames.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace InterfurCreations.AdventureGames.WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _config;

        public Worker(ILogger<Worker> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Log.LogMessage("Bot service started and running. Calling Main method.");
            ProgramRun.Run(_config);
        }
    }
}
