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
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights;
using BotCustomConnectorSvc.ErrorHandler;
using BotCustomConnectorSvc.Helpers;

namespace BotCustomConnectorSvc.Controllers
{
    [RoutePrefix("v3/conversations")]
    public class ConversationController : Controller
    {
        TelemetryClient telemetry = new TelemetryClient();

        [HttpDelete]
        [Route("clearcache")]
        public string Delete()
        {
           return CacheHelper.ClearAllConvData();
        }

        [HttpDelete]
        [Route("{conversationId}")]
        public string DeleteConv(string conversationId)
        {
            return CacheHelper.ClearConvData(conversationId);
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
            return JsonConvert.DeserializeObject<object>(
                $"{{\"conversationId\":\"{conversationId}\",\"token\":\"{conversationId}\",\"expires_in\":1.7976931348623157e+308,\"streamUrl\":\"\"}}");
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
            return Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                activities = conv.Activities.Values.ToList<Activity>(),
                watermark = conv.Activities.Count > 0 ? conv.Activities.Keys.Max() : 0
            });
        }

        /// <summary>
        /// Returns id from received activity
        /// </summary>
        /// <param name="conversationId"></param>
        /// <param name="activity"></param>
        /// <returns></returns>

        [HttpPost]
        [Route("{conversationId}/activities")]
        public async Task<object> Post(string conversationId, [System.Web.Http.FromBody] Activity activity)
        {
            if (activity != null)
            {
                CacheHelper.GetActivityId(conversationId, activity);
                activity.Conversation = new ConversationAccount() { Id = conversationId };
                CacheHelper.WriteConversationActivityToStorage(conversationId, activity);

                if (Helper.PostToBotEnabled && string.IsNullOrEmpty(activity.ReplyToId))
                {
                    bool status = await PostToBot(conversationId, activity);

                    if (!status)
                    {
                        Response.StatusCode = 500;
                        return Newtonsoft.Json.JsonConvert.DeserializeObject<object>($"{{\"id\":\"Post to bot failed\"}}");
                    }
                }

                Response.StatusCode = 200;
                return Newtonsoft.Json.JsonConvert.DeserializeObject<object>($"{{\"id\":\"{activity.Id}\"}}");
            }
            
            return null;
        }

        [HttpPost]
        [Route("{conversationId}/activities/{activityId}")]
        public string Post(string conversationId, string activityId, [System.Web.Http.FromBody] Activity activity)
        {
            if (activity.Timestamp == DateTime.MinValue)
            {
                activity.Timestamp = DateTime.UtcNow;
            }

            if (string.IsNullOrEmpty(activity.Id))
            {
                CacheHelper.GetActivityId(conversationId, activity);
            }

            if (activity.InternalId == 0)
            {
                activity.InternalId = int.Parse(activity.Id.Split('|')[1]);
            }

            if (activity.Conversation == null || string.IsNullOrEmpty(activity.Conversation.Id))
            {
                activity.Conversation = new ConversationAccount() { Id = conversationId };
            }

            CacheHelper.WriteConversationActivityToStorage(conversationId, activity);

            //var res = new Dictionary<string, string>()
            //{
            //    {"id", activity.Id}
            //};

            //return Newtonsoft.Json.JsonConvert.SerializeObject(res);
            return $"{{\"id\":\"{activity.Id}\"}}";
        }

        private async Task<bool> PostToBot(string conversationId, Activity activity)
        {
            if (HttpContext.Request.Headers.AllKeys.Contains("BotBaseAddress") && HttpContext.Request.Headers.AllKeys.Contains("Authorization"))
            {
                string botBaseAddress = HttpContext.Request.Headers["BotBaseAddress"];
                string authorization = HttpContext.Request.Headers["Authorization"];
                authorization = authorization.Remove(0, 7);
                try
                {
                    using (HttpClient _client = new HttpClient { BaseAddress = new Uri(botBaseAddress) })
                    {
                        _client.DefaultRequestHeaders.Accept.Clear();
                        _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authorization);

                        string jsonInput = JsonConvert.SerializeObject(activity);
                        StringContent strContent = new StringContent(jsonInput, Encoding.UTF8, "application/json");
                        // Send Message
                        var response = await _client.PostAsync("api/messages", strContent);
                        return response.StatusCode == System.Net.HttpStatusCode.OK;
                    }
                }
                catch
                {

                }
            }

            return false;
        }
    }
}
