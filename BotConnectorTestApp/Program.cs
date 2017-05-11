using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BotConnectorTestApp
{
    class Program
    {
        static string botSvcAddress = ConfigurationManager.AppSettings.Get("BotSvcAddress");
        static string botId = ConfigurationManager.AppSettings.Get("BotId");
        static int watermark = 0, prevWatermark = 0;
        static bool quit = false;

        static void Main(string[] args)
        {
            // Getting conversation ID from dummy service
            string conversationId = GetConversationId();

            var task1 = Task.Factory.StartNew(() => CheckMessages(conversationId));
            var task2 = Task.Factory.StartNew(() => Run(conversationId));

            Task.WaitAll(task1, task2);
        }

        static async Task Run(string conversationId)
        {
            string message;
            while (!string.IsNullOrEmpty(message = Console.ReadLine()))
            {
                await PostMessageToBot(conversationId, message);
            }

            quit = true;
        }

        static async Task CheckMessages(string conversationId)
        {
            while (!quit)
            {
                DisplayConversationMessages(conversationId);
                Thread.Sleep(2000);
            }
        }

            private static string GetConversationId()
        {
            // Getting conversation ID from dummy service
            HttpClient _client = new HttpClient();
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.BaseAddress = new Uri(botSvcAddress);
            var response = _client.PostAsync("v3/conversations", null).Result;

            var responseJson = JsonConvert.DeserializeObject<Conv>(response.Content.ReadAsStringAsync().Result);
            return responseJson.ConversationId;
        }

        private static async Task<bool> PostMessageToBot(string conversationId, string msg)
        {
            // Sending interactions and checking for accepted
            HttpClient _client = new HttpClient { BaseAddress = new Uri(botSvcAddress) };
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));            
            var activity = new Activity
            {
                ChannelData = new Dictionary<string, string>()
                {
                    {"clientActivityId", Guid.NewGuid().ToString().Replace("-", "")}
                },
                ChannelId = "custom",
                Conversation = new ConversationAccount()
                {
                    Id = conversationId
                },
                From = new ChannelAccount("default-user", "User"),
                Id = Guid.NewGuid().ToString().Replace("-", ""),
                Locale = "en-US",
                LocalTimestamp = DateTime.Now.ToLocalTime(),
                Recipient = new ChannelAccount(botId, "Bot"),
                //ServiceUrl = botSvcAddress,
                Timestamp = DateTime.Now.ToUniversalTime(),
                Text = msg,
                Type = "message"
            };

            string jsonInput = JsonConvert.SerializeObject(activity);
            StringContent strContent = new StringContent(jsonInput, Encoding.UTF8, "application/json");
            // Send Message
            var response = await _client.PostAsJsonAsync($"v3/conversations/{conversationId}/activities", activity);
            Trace.WriteLine(response.StatusCode);
            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(response.StatusCode.ToString());
            }

            return true;
        }

        private static async void DisplayConversationMessages(string conversationId)
        {
            HttpClient _client = new HttpClient();
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.BaseAddress = new Uri(ConfigurationManager.AppSettings.Get("BotSvcAddress"));
            string url = $"v3/conversations/{conversationId}/activities";

            if (watermark > 0)
            {
                url += $"/{watermark}";
            }
            var response = _client.GetAsync(url).Result;

            var conversation = JsonConvert.DeserializeObject<Conversation1>(response.Content.ReadAsStringAsync().Result);

            if (conversation != null && conversation.Activities.Count > 0)
            {
                int lastWatermark = int.Parse(conversation.watermark);
                if (lastWatermark > watermark)
                {
                    foreach (Activity a in conversation.Activities)
                    {
                        Console.WriteLine(JsonConvert.SerializeObject(a));
                        Console.WriteLine("----------------------------------");
                    }

                    watermark = lastWatermark;
                }
            }
        }
    }
}
