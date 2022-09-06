using Newtonsoft.Json;
using InterfurCreations.AdventureGames.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Threading;
using InterfurCreations.AdventureGames.Logging;

namespace InterfurCreations.AdventureGames.Telegram
{
    public class TelegramService
    {

        private readonly string _telegramUrl;
        private readonly string _botToken;

        private readonly string _url;


        public TelegramService(string telegramUrl, string botToken)
        {
            _telegramUrl = telegramUrl;
            _botToken = botToken;
            _url = _telegramUrl + _botToken;

        }

        public async Task<T> PostRequestAsync<T>(string methodName, object data)
        {
            T returnedObject = default(T);
            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromMilliseconds(Timeout.Infinite);
                    var jsonData = JsonConvert.SerializeObject(data);

                    var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                    var fullString = _url + "/" + methodName;

                    Log.LogMessage("Quering: " + fullString, LogType.Verbose);
                    Log.LogMessage("With Data: " + jsonData, LogType.Verbose);

                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, fullString)
                    {
                        Content = content,
                    };
                    request.Headers.Authorization = new AuthenticationHeaderValue("furryadventurebot", _botToken);

                    var response = await client.SendAsync(request);

                    var responseString = await response.Content.ReadAsStringAsync();
                    returnedObject = JsonConvert.DeserializeObject<T>(responseString);

                    if (!response.IsSuccessStatusCode)
                    {
                        Log.LogMessage(response.ReasonPhrase, LogType.Error);
                        Log.LogMessage(responseString, LogType.Error);
                    }

                    Log.LogMessage(responseString, LogType.Verbose);
                }
            }
            catch (Exception e) { Log.LogMessage("There was an error with sending a HTML query to telegram: " + e.UnwrapException(), LogType.Error); }
            return returnedObject;
        }
    }
}
