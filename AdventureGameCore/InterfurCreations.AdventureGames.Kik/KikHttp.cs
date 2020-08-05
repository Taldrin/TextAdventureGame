using InterfurCreations.AdventureGames.Configuration;
using InterfurCreations.AdventureGames.Logging;
using InterfurCreations.AdventureGames.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.Kik
{
    public class KikHttp
    {
        private readonly string _kikUrl;
        private readonly string _botApiKey;
        private readonly HttpClient _client;

        public KikHttp(string kikUrl, string botApiKey)
        {
            _kikUrl = kikUrl;
            _botApiKey = botApiKey;
            _client = new HttpClient();
            var kikBotName = new ConfigurationService().GetConfig("KikBotName");
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{kikBotName}:{_botApiKey}")));
        }

        public async Task<T> GetRequestAsync<T>(string methodName)
        {
            T returnedObject = default(T);
            try
            {
                var fullString = _kikUrl + "/" + methodName;

                Log.LogMessage("Quering: " + fullString, LogType.Verbose);

                var response = await _client.GetAsync(fullString);

                var responseString = await response.Content.ReadAsStringAsync();
                returnedObject = JsonConvert.DeserializeObject<T>(responseString);

                if (!response.IsSuccessStatusCode)
                {
                    Log.LogMessage(response.ReasonPhrase, LogType.Error);
                    Log.LogMessage(responseString, LogType.Error);
                }

                Log.LogMessage(responseString, LogType.Verbose);
            }
            catch (Exception e) { Log.LogMessage("There was an error with sending a HTML query to telegram " + e.Message, LogType.Error); }
            return returnedObject;
        }

        public async Task<T> PostRequestAsync<T>(string methodName, object data)
        {
            T returnedObject = default(T);
            try
            {
                var jsonData = JsonConvert.SerializeObject(data);

                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                var fullString = _kikUrl + "/" + methodName;

                Log.LogMessage("Quering: " + fullString, LogType.Verbose);
                Log.LogMessage("With Data: " + jsonData, LogType.Verbose);

                var response = await _client.PostAsync(fullString, content);

                var responseString = await response.Content.ReadAsStringAsync();
                returnedObject = JsonConvert.DeserializeObject<T>(responseString);

                if (!response.IsSuccessStatusCode)
                {
                    Log.LogMessage(response.ReasonPhrase, LogType.Error);
                    Log.LogMessage(responseString, LogType.Error);
                }

                Log.LogMessage(responseString, LogType.Verbose);
            }
            catch (Exception e) { Log.LogMessage("There was an error with sending a HTML query to telegram " + e.Message, LogType.Error); }
            return returnedObject;
        }

        public async Task PostRequestAsync(string methodName, object data)
        {
            try
            {
                var jsonData = JsonConvert.SerializeObject(data);

                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                var fullString = _kikUrl + "/" + methodName;

                Log.LogMessage("Quering: " + fullString, LogType.Verbose);
                Log.LogMessage("With Data: " + jsonData, LogType.Verbose);

                var response = await _client.PostAsync(fullString, content);

                var responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Log.LogMessage(response.ReasonPhrase, LogType.Error);
                    Log.LogMessage(responseString, LogType.Error);
                }

                Log.LogMessage(responseString, LogType.Verbose);
            }
            catch (Exception e) { Log.LogMessage("There was an error with sending a HTML query to telegram " + e.Message, LogType.Error); }
        }
    }
}
