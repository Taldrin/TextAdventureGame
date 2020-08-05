using InterfurCreations.AdventureGames.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Services
{
    public static class ServiceStore
    {
        public static IWebhookMessageHandlerService WebhookMessageHandlerService { get; set; }
        public static object HttpLockObj { get; set; }
    }
}
