using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using InterfurCreations.AdventureGames.Services.Interfaces;
using InterfurCreations.AdventureGames.Services;
using System;
using Newtonsoft.Json;

namespace InterfurCreations.AdventureGames.WebhookService
{
    public class WebhookMessageHandlerService : IWebhookMessageHandlerService
    {
        private readonly List<IWebhookHandler> _handlers;

        public WebhookMessageHandlerService(IEnumerable<IWebhookHandler> handlers)
        {
            _handlers = handlers.ToList();
            ServiceStore.WebhookMessageHandlerService = this;
        }

        public void HandleMessage(JObject obj)
        {
            bool handled = false;
            foreach (var handler in _handlers)
            {
                var type = handler.ObjectType;
                try
                {
                    var castedObj = obj.ToObject(type);
                    if (castedObj != null && handler.IsValid(castedObj))
                    {
                        handler.Handle(castedObj, obj);
                        handled = true;
                    }
                } catch (ArgumentException e)
                {
                }
            }
        }
    }
}
