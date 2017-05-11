using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using System.Web.Mvc;
using BotCustomConnectorSvc.Models;
using System.Configuration;
using System.Net;

namespace BotCustomConnectorSvc.Controllers
{
    [RoutePrefix("v3/conversations")]
    public class ConversationController : Controller
    {        
        string botBaseAddress, appId, appKey, channelId = "emulator", botId;

        App app;

        public ConversationController()
        {
            botBaseAddress = ConfigurationManager.AppSettings["BotBaseAddress"];
            appId = ConfigurationManager.AppSettings["AppId"];
            appKey = ConfigurationManager.AppSettings["Appkey"];
            botId = ConfigurationManager.AppSettings["BotId"];
            app = new App() { AppId = appId, AppKey = appKey };
        }

        /// <summary>
        /// Return a new conversation id, token and expiration
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public object Post()
        {
            var conversationId = Guid.NewGuid().ToString().Replace("-", "");
            return JsonConvert.DeserializeObject<object>($"{{\"conversationId\":\"{conversationId}\",\"token\":\"{conversationId}\",\"expires_in\":1.7976931348623157e+308,\"streamUrl\":\"\"}}");
        }

        [HttpGet]
        [Route("")]
        public object Get()
        {
            return "test";
        }

        [HttpGet]
        [Route("{conversationId}/activities")]
        [Route("{conversationId}/activities/{watermark}")]
        
        public string GetData(string conversationId, string watermark = "0")
        {
            Conversation conv = CacheHelper.GetConversation(conversationId, int.Parse(watermark));
            return Newtonsoft.Json.JsonConvert.SerializeObject(new { activities = conv.Activities.Values.ToList<Activity>(), watermark = conv.Activities.Count > 0 ? conv.Activities.Keys.Max() : 0 });
        }

        /// <summary>
        /// Returns id from received activity
        /// </summary>
        /// <param name="conversationId"></param>
        /// <param name="activity"></param>
        /// <returns></returns>

        [HttpPost]
        [Route("{conversationId}/activities")]
        public async Task<object> Post(string conversationId, [System.Web.Http.FromBody]Activity activity)
        {
            if (activity != null)
            {
                activity.ChannelId = channelId;
                activity.ChannelData = new Dictionary<string, string>()
                {
                    {"clientActivityId", Guid.NewGuid().ToString().Replace("-", "")}
                };
                activity.Conversation = new ConversationAccount()
                {
                    Id = conversationId
                };
                activity.Locale = "en-US";
                activity.ServiceUrl = $"{this.ControllerContext.HttpContext.Request.Url.Scheme}://{this.ControllerContext.HttpContext.Request.Url.Authority}";
                activity.Recipient = new ChannelAccount(botId, "Bot");
                activity.Timestamp = DateTime.Now.ToUniversalTime();

                CacheHelper.UpdateConversation(conversationId, activity);                
                await PostToBot(conversationId, activity);
 
                return Newtonsoft.Json.JsonConvert.DeserializeObject<object>($"{{\"id\":\"{activity.Id}\"}}");
            }

            return null;
        }

        [HttpPost]
        [Route("{conversationId}/activities/{activityId}")]
        public string Post(string conversationId, string activityId, [System.Web.Http.FromBody]Activity activity)
        {
            activity.Timestamp = DateTime.Now.ToUniversalTime();
            CacheHelper.UpdateConversation(conversationId, activity);

            var res = new Dictionary<string, string>()
            {
                {"id",  activity.Id}
            };
            
            return Newtonsoft.Json.JsonConvert.SerializeObject(res);
        }

        private async Task PostToBot(string conversationId, Activity activity)
        {            
            HttpClient _client = new HttpClient { BaseAddress = new Uri(botBaseAddress) };
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            string token = Helpers.GetJwtToken(app);
            if (!string.IsNullOrEmpty(token))
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            string jsonInput = JsonConvert.SerializeObject(activity);
            StringContent strContent = new StringContent(jsonInput, Encoding.UTF8, "application/json");
            // Send Message
            var response = await _client.PostAsync("api/messages", strContent);            
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string status = "Ok";
            }
        }
    }
}
