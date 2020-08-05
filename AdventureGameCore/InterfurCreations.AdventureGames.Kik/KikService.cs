using InterfurCreations.AdventureGames.Configuration;
using InterfurCreations.AdventureGames.Kik.KikObjects;
using InterfurCreations.AdventureGames.Kik.KikObjects.Send;
using InterfurCreations.AdventureGames.Logging;
using InterfurCreations.AdventureGames.Services;
using InterfurCreations.AdventureGames.Services.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Kik
{
    public class KikService : ICommunicator
    {
        private IWebhookService _webhookService;

        public static KikHttp _kikHttp;

        public KikService(IWebhookService webhookService, IWebhookMessageHandlerService messageHandlerService)
        {
            _webhookService = webhookService;
        }

        public async void SetupAsync()
        {
            Log.LogMessage("Starting up Kik bot");
            var kikApi = new ConfigurationService().GetConfig("KikApiKey");
            _kikHttp = new KikHttp("https://api.kik.com/v1", kikApi);
            string webhookUrl = "";
            try
            {
                webhookUrl = _webhookService.StartWebhookService();
            } catch (Exception e)
            {
                Log.LogMessage("There was a critical error starting up the webhook service: " + e.Message, LogType.Error, e.StackTrace);
                throw e;
            }

            var kikConfig = new KikConfigDataObject
            {
                webhook = webhookUrl,
                features = new KikFeaturesDataObject()
            };

            Log.LogMessage("Sending config to KIK");
            try
            {
                await KikMethods.SendConfigAsync(_kikHttp, kikConfig);
                Log.LogMessage("KIK bot setup and ready!");
            } catch (Exception e)
            {
                Log.LogMessage("There was a critical error sending initial config to KIK: " + e.Message, LogType.Error, e.StackTrace);
            }
        }
    }
}
