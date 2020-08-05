using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace InterfurCreations.AdventureGames.Webhook
{
    public class WebHoster
    {

        public static void StartWebHost(int port) =>
            WebHost.CreateDefaultBuilder(new string[0])
                .UseStartup<Startup>().UseUrls($"https://*:{port}").Build().Run();
    }
}
