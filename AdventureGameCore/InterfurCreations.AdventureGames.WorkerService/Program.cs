using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;

namespace InterfurCreations.AdventureGames.WorkerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(config =>
            {
                var connection = Environment.GetEnvironmentVariable("ConnectionString");
                config.AddAzureAppConfiguration(connection);
            })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
            }).UseWindowsService();
    }
}
