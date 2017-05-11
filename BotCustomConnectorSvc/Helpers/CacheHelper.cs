using BotCustomConnectorSvc.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BotCustomConnectorSvc
{
    public static class Helpers
    {
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
    }

    public static class CacheHelper
    {
        static int cacheSize = 2000;
        static StateDataDictionary stateDictionary = new StateDataDictionary(cacheSize);
        public static StateDataDictionary StateDictionary
        {
            get
            {
                return stateDictionary;
            }
        }

        static Conversations conversations = new Conversations(cacheSize);
        public static Conversations Conversations
        {
            get
            {
                return conversations;
            }
        }

        public static int UpdateConversation(string conversationId, Activity activity)
        {
            Conversation conv = default(Conversation);

            if (CacheHelper.Conversations.ContainsKey(conversationId))
            {
                conv = CacheHelper.Conversations[conversationId];
            }
            else
            {
                conv = new Conversation() { Id = conversationId };
                CacheHelper.Conversations.Add(conversationId, conv);
            }

            int sequence = conv.Activities.Count > 0 ? conv.Activities.Keys.Max(): 0;
            activity.Id = $"{conversationId}|{++sequence}";
            conv.Activities.Add(sequence, activity);

            return sequence;
        }

        public static Conversation GetConversation(string conversationId, int watermark)
        {
            Conversation conv = new Conversation();

            if (CacheHelper.Conversations.ContainsKey(conversationId))
            {
                conv = CacheHelper.Conversations[conversationId];
            }

            if (watermark > 0)
            {
                Conversation outPut = new Conversation() { Id = conversationId };
                foreach (KeyValuePair<int, Activity> act in conv.Activities.Where(a => a.Key > watermark))
                {
                    outPut.Activities.Add(act.Key, act.Value);
                }

                return outPut;
            }
            else
            {
                return conv;
            }
        }

        private static void UpdateState(string key, StateData data)
        {           
            if (CacheHelper.StateDictionary.ContainsKey(key))
            {
                CacheHelper.StateDictionary[key] = data;
            }
            else
            {
                CacheHelper.StateDictionary.Add(key, data);
            }
        }

        public static void UpdateConversationState(string conversationId, StateData data)
        {
            UpdateState(conversationId, data);
        }

        public static void UpdateConversationUserState(string conversationId, string userId, StateData data)
        {
            string key = $"{conversationId}_{userId}";
            UpdateState(key, data);
        }

        public static void UpdateUserState(string userId, StateData data)
        {
            UpdateState(userId, data);
        }

        private static StateData GetStateData(string key)
        {
            StateData stateData = new StateData();

            if (CacheHelper.StateDictionary.ContainsKey(key))
            {
                stateData = CacheHelper.StateDictionary[key];
            }

            return stateData;
        }

        public static StateData GetConversationUserState(string conversationId, string userId)
        {
            string key = $"{conversationId}_{userId}";
            return GetStateData(key);
        }

        public static StateData GetConversationState(string conversationId)
        {
            return GetStateData(conversationId);
        }

        public static StateData GetUserState(string userId)
        {
            return GetStateData(userId);
        }
    }
}
