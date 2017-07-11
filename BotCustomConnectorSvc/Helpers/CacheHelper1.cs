using BotCustomConnectorSvc.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BotCustomConnectorSvc.Helpers;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Table.Queryable;

namespace BotCustomConnectorSvc
{
    public static class CacheHelper
    {
        static bool dataCleanupEnabled = bool.Parse(ConfigurationManager.AppSettings["DataCleanupEnabled"]);

        public static int GetActivityId(string conversationId, Activity activity)
        {
            Activity lastActivity = RedisHelper.GetLastActivity(conversationId);

            int sequence = 0;
            if (lastActivity != null)
            {
                sequence = lastActivity.InternalId;
            }

            activity.Id = $"{conversationId}|{++sequence}";
            activity.InternalId = sequence;

            if (sequence == 1)
            {
                RedisHelper.SaveConversation(new Conversation() { Id = conversationId});
            }

            return sequence;
        }

        public static Activity GetConversationActivity(string conversationId, int activityId)
        {
            return RedisHelper.GetActivity(conversationId, activityId);
        }

        public static Conversation GetConversation(string conversationId, int watermark = 0)
        {

            List<Activity> activities = RedisHelper.GetAllActivities(conversationId);

            Conversation outPut = new Conversation() {Id = conversationId};

            if (activities.Count > 0)
            {
                foreach (Activity act in (watermark > 0
                    ? activities.Where(a => a.InternalId >= watermark)
                    : activities))
                {
                    outPut.Activities[act.InternalId] = act;
                }
            }

            return outPut;
        }

        public static void UpdateConversationState(string conversationId, StateData data)
        {
            RedisHelper.SaveStateData(conversationId, data);
        }

        public static void UpdateConversationUserState(string conversationId, string userId, StateData data)
        {
            string key = $"{conversationId}_{userId}";
            RedisHelper.SaveStateData(key, data);
        }

        public static void UpdateUserState(string userId, StateData data)
        {            
            RedisHelper.SaveStateData(userId, data);
        }

        private static StateData GetStateData(string key)
        {
            return ReadStateFromStorage(key);
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

        public static string ClearAllConvData()
        {
            if (dataCleanupEnabled)
            {
                DeleteAllConvFromStorage();
                return $"conversation data deleted";
            }
            else
            {
                return "Data cleanup disabled";
            }
        }

        public static string ClearConvData(string convId)
        {
            if (dataCleanupEnabled)
            {
                DeleteConversationFromStorage(convId);
                return $"conversation data deleted";
            }
            else
            {
                return "Data cleanup disabled";
            }
        }

        public static string ClearAllConvStateData()
        {
            if (dataCleanupEnabled)
            {
                ClearAllConvStateData();
                return $"State Data deleted";
            }
            else
            {
                return "Data cleanup disabled";
            }
        }

        public static string ClearConvStateData(string convId)
        {
            if (dataCleanupEnabled)
            {
                DeleteStateDataFromStorage(convId);
                return "State data deleted";
            }
            else
            {
                return "Data cleanup disabled";
            }
        }

        public static void WriteConversationActivityToStorage(string convId, Activity activity)
        {
            RedisHelper.SaveActivity(convId, activity);
        }

        private static void DeleteAllConvFromStorage()
        {
            RedisHelper.FlushConversations();
        }

        private static void DeleteConversationFromStorage(string convId)
        {
            RedisHelper.DeleteConversation(convId);
        }

        private static StateData ReadStateFromStorage(string key)
        {
            return RedisHelper.GetStateData(key);            
        }

        private static void DeleteStateDataFromStorage(string key)
        {
            RedisHelper.DeleteStateData(key);
        }
    }

    public class ExcpetionEntity1
    {
        public string Path { get; set; }

        public string Message { get; set; }

        public string StackTrace { get; set; }

        public DateTime TimeStamp { get; set; }
    }
}
