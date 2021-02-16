using InterfurCreations.AdventureGames.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InterfurCreations.AdventureGames.LanguageTool
{
    public class LanguageToolCommunicator
    {
        private readonly string _baseUrl;

        public LanguageToolCommunicator(string baseUrl)
        {
            _baseUrl = baseUrl;    
        }

        public async Task<T> PostRequest<T>(string methodName, Dictionary<string, string> formData)
        {
            T returnedObject = default(T);
            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromMilliseconds(Timeout.Infinite);

                    var content = new FormUrlEncodedContent(formData);

                    var fullString = _baseUrl + "/" + methodName;

                    Log.LogMessage("Quering: " + fullString, LogType.Verbose);

                    var response = await client.PostAsync(fullString, content);

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
            catch (Exception e) { Log.LogMessage("There was an error with sending a HTML query to Language Tool " + e.Message, LogType.Error); }
            return returnedObject;
        }
    }
}
