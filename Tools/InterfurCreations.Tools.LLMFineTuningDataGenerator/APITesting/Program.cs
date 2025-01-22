using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace APITesting
{
    internal class Program
    {
        static HttpClient client;
        static string completeConvo;
        static void Main(string[] args)
        {
            client = new HttpClient();
            client.BaseAddress = new Uri(@"https://p36288cf7rrikq7g.eu-west-1.aws.endpoints.huggingface.cloud");

            ConvoRunner();
        }

        static void ConvoRunner()
        {
            while(true)
            {
                Console.Write(">>: ");
                var line = Console.ReadLine();

                line = $"<s>[INST] {line} [/INST] ";

                completeConvo = $"{completeConvo}{line}";

                string completeCompletion = "";
                while (true)
                {
                    var completion = SendReq($"{completeConvo}{completeCompletion}");
                    completeCompletion = $"{completeCompletion}{completion}";

                    if (completion.Contains("[5]") || completion.Contains("</s>"))
                        break;
                }
                completeConvo = $"{completeConvo}{completeCompletion}</s>";

                Console.WriteLine(completeCompletion);
            }
        }

        static string SendReq(string message)
        {
            var textData = JsonConvert.SerializeObject(new HuggingFaceInfRequest
            {
                inputs = message,
                parameters = new HuggingFaceInfParameters
                {
                    max_length = 1028,
                    repetition_penalty = null,
                    temperature = null,
                    bad_words_ids = new List<List<int>>(),
                    top_p = null,
                    max_new_tokens = 50
                }
            }); 

            var req = new HttpRequestMessage(HttpMethod.Post, "");
            req.Content = new StringContent(textData, Encoding.UTF8, "application/json");
            //req.Headers.Add("Content-Type", "application/json");

            var response = client.Send(req);

            var responseString = response.Content.ReadAsStringAsync().Result;

            var responseData = JsonConvert.DeserializeObject<List<HuggingFaceResponseMessage>>(responseString);

            return responseData.First().generated_text;
        }
    }

    public class HuggingFaceInfRequest
    {
        public string inputs { get; set; }
        public HuggingFaceInfParameters parameters { get; set; }
    }

    public class HuggingFaceInfParameters
    {
        public double? repetition_penalty { get; set; }
        public int? max_length { get; set; }
        public float? temperature { get; set; }
        public int? top_k { get; set; }
        public float? top_p { get; set; }
        public float? typical_p { get; set; }
        public int max_new_tokens { get; set; }
        public List<List<int>> bad_words_ids { get; set; }
    }

    public class HuggingFaceResponseMessage
    {
        public string generated_text { get; set; }
    }
}
