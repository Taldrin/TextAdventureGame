using InterfurCreations.AdventureGames.Logging;
using InterfurCreations.AdventureGames.Services;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.HeartbeatMonitor
{
    public class HeartbeatMonitorService : IHeartbeatMonitor
    {
        private string _heartbeatUrl;

        public void BeginMonitor(string heartbeatUrl)
        {
            _heartbeatUrl = heartbeatUrl;
            StartMonitorThread();
        }

        private void StartMonitorThread()
        {
            if (!string.IsNullOrEmpty(_heartbeatUrl))
            {
                Thread th = new Thread(async a =>
                {
                    while (true)
                    {
                        try
                        {
                            await SendHeartbeat();
                            Thread.Sleep(30000);
                        }
                        catch (Exception) { }
                    }
                });

                th.Start();
            }
        }

        private async Task SendHeartbeat()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(_heartbeatUrl);
                    
                    var responseString = await response.Content.ReadAsStringAsync();
                    
                   // if (!response.IsSuccessStatusCode)
                   // {
                   //     Log.LogMessage(response.ReasonPhrase, LogType.Error);
                   //     Log.LogMessage(responseString, LogType.Error);
                   // }
                   // Log.LogMessage(responseString, LogType.Verbose);
                }
            }
            catch (Exception e) { Log.LogMessage("There was an error with sending a HTML query to Heartbeat monitor " + e.Message, LogType.Error); }
        }
    }
}
