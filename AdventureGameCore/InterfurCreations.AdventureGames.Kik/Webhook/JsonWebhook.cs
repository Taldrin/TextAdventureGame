using InterfurCreations.AdventureGames.Services;
using InterfurCreations.AdventureGames.Services.Interfaces;
using Microsoft.AspNet.WebHooks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace InterfurCreations.AdventureGames.Kik.Webhook
{
    public class JsonWebhook : WebHookHandler
    {
        private IWebhookMessageHandlerService _webhookMessageHandlerService;

        public JsonWebhook()
        {
            this.Receiver = GenericJsonWebHookReceiver.ReceiverName;
        }

        public override Task ExecuteAsync(string receiver, WebHookHandlerContext context)
        {
            if(context.TryGetData<JObject>(out var value))
            {
                ServiceStore.WebhookMessageHandlerService.HandleMessage(value);
            }
            return Task.FromResult(true);
        }
    }
}