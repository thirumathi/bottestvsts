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

namespace BotCustomConnectorSvc.Controllers
{
    [RoutePrefix("v3/botstate/emulator")]
    public class StateController : Controller
    {
        [HttpDelete]
        [Route("conversations")]
        public bool Delete()
        {
            CacheHelper.StateDictionary.Clear();
            return true;
        }

        [HttpDelete]
        [Route("conversations/{conversationId}")]
        public string DeleteConv(string conversationId)
        {
            List<string> keys = CacheHelper.StateDictionary.Keys.Where(k => k.StartsWith(conversationId)).ToList();
            foreach(string key in keys)
            {
                CacheHelper.StateDictionary.Remove(key);
            }
            return $"{keys.Count} items deleted";
        }

        [HttpGet]
        [Route("conversations/{id}")]
        public string GetConversations(string id)
        {
            StateData data = CacheHelper.GetConversationState(id);
            return Newtonsoft.Json.JsonConvert.SerializeObject(data);
        }

        [HttpGet]
        [Route("conversations/{conversationId}/users/{userId}")]
        public string Get(string conversationId, string userId)
        {
            StateData data = CacheHelper.GetConversationUserState(conversationId, userId);
            return Newtonsoft.Json.JsonConvert.SerializeObject(data);
        }

        [HttpGet]
        [Route("users/{userId}")]
        public string GetUsers(string userId)
        {
            StateData data = CacheHelper.GetUserState(userId);
            return Newtonsoft.Json.JsonConvert.SerializeObject(data);
        }

        [HttpPost]
        [Route("conversations/{conversationId}")]
        public string ConvDataPost(string conversationId, [System.Web.Http.FromBody]StateData stateData)
        {
            if (stateData != null)
            {
                CacheHelper.UpdateConversationState(conversationId, stateData);
                return Newtonsoft.Json.JsonConvert.SerializeObject(stateData);
            }

            return string.Empty;
        }


        [HttpPost]
        [Route("conversations/{conversationId}/users/{userId}")]
        public string ConvDataPost(string conversationId, string userId, [System.Web.Http.FromBody]StateData stateData)
        {
            if (stateData != null)
            {
                CacheHelper.UpdateConversationUserState(conversationId, userId, stateData);
                return Newtonsoft.Json.JsonConvert.SerializeObject(stateData);
            }

            return string.Empty;
        }

        [HttpPost]
        [Route("users/{userId}")]
        public string UserDataPost(string userId, [System.Web.Http.FromBody]StateData stateData)
        {
            if (stateData != null)
            {
                CacheHelper.UpdateUserState(userId, stateData);
                return Newtonsoft.Json.JsonConvert.SerializeObject(stateData);
            }

            return string.Empty;
        }
    }
}
