using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Services.Interfaces
{
    public interface IWebhookMessageHandlerService
    {
        void HandleMessage(JObject obj);
    }
}
