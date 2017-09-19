using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HelperLib
{
    public static class Helpers
    {

        public static string AccessToken { get; set; }
        public static string ConvId { get; set; }

        static int userId = 0;
        static int counter = 0;
        static Token token;
        public static string GetJwtToken(App app)
        {
            if (string.IsNullOrEmpty(app.AppId) || string.IsNullOrEmpty(app.AppKey))
            {
                return string.Empty;
            }

            if (token == null || token.ExpiryUtc < DateTime.UtcNow)
            {
                HttpClient _client = new HttpClient();
                _client.DefaultRequestHeaders.Accept.Clear();
                _client.BaseAddress = new Uri("https://login.microsoftonline.com/");
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "client_credentials"),
                    new KeyValuePair<string, string>("client_id", app.AppId),
                    new KeyValuePair<string, string>("client_secret", app.AppKey),
                    new KeyValuePair<string, string>("scope", "https://graph.microsoft.com/.default")
                });
                var response = _client.PostAsync("/botframework.com/oauth2/v2.0/token", content).Result;
                token = JsonConvert.DeserializeObject<Token>(response.Content.ReadAsStringAsync().Result);
                token.ExpiryUtc = DateTime.UtcNow.AddSeconds(3599);
            }

            return token.Access_token;
        }

        public static string GetChannelId(bool isMixed)
        {
            if (!isMixed)
            {
                return "emulator";
            }

            counter++;

            switch (counter % 4)
            {
                case 0:
                    counter = 0;
                    return "emulator";
                case 1:
                    return "webchat";
                case 2:
                    return "directline";
                case 3:
                    return "skype";
            }

            return "emulator";
        }

        public static string GetUserId()
        {
            counter++;

            //return "10865455";
            return $"user{counter}";
        }
    }
}
