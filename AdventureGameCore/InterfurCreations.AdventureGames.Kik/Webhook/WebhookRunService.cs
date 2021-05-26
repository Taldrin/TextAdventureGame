using InterfurCreations.AdventureGames.Logging;
using InterfurCreations.AdventureGames.Services;
using InterfurCreations.AdventureGames.Webhook;
using Microsoft.Owin.Hosting;
using System.Net;

namespace InterfurCreations.AdventureGames.Kik.Webhook
{
    public class WebhookRunService : IWebhookService
    {
        public string StartWebhookService(int port = 44368)
        {
            string externalip = new WebClient().DownloadString("http://icanhazip.com").TrimEnd('\n');

            Log.LogMessage("Identified IP as: " + externalip);

            var options = new StartOptions();
            options.Port = port;
            options.Urls.Add($"https://*:{port}");

            Log.LogMessage("Starting up webhook owin reciever");
            WebApp.Start<Startup>(options);

            string webhookUrl = $"https://{externalip}:{port}/api/webhooks/incoming/";
            Log.LogMessage("Webhook reciever started, with webhook URL: " + webhookUrl);
            return webhookUrl;
        }
    }
}